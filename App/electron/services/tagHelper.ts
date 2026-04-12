// Port of TagHelper.cs - handles the ~token~ export format
// e.g. ~StartedOnTicks~^~Task~^~Comments~^~Tag[Name=Bill%].Description~^~Duration~

export interface TagKey {
	key: string;
	index?: number | null;
	indexPropertyName?: string | null;
	indexPropertyValue?: string | null;
	propertyName?: string | null;
}

export function parseTagKey(raw: string): TagKey | null {
	if (!raw) return null;

	const tk: TagKey = { key: raw };

	const match = raw.match(/^(\w+)(?:\[(\d+)\](?:\.(\w+))?|\[(\w+)=([^\]]+)\](?:\.(\w+))?)?$/);
	if (match) {
		tk.key = match[1];
		tk.index = match[2] != null ? parseInt(match[2]) : null;
		tk.propertyName = match[3] ?? match[6] ?? null;
		tk.indexPropertyName = match[4] ?? null;
		tk.indexPropertyValue = match[5] ?? null;
	}

	return tk;
}

export function getTagsFromFormat(input: string): string[] {
	return [...input.matchAll(/~([^~]+)~/g)].map((m) => m[1]);
}

type TagResolver = (tagKey: TagKey, task: Record<string, unknown>) => string;

const registry = new Map<string, TagResolver>();

export function registerTag(name: string, resolver: TagResolver): void {
	if (!registry.has(name)) registry.set(name, resolver);
}

export function runTag(rawTag: string, task: Record<string, unknown>): string | null {
	const tagKey = parseTagKey(rawTag);
	if (!tagKey) return null;
	const resolver = registry.get(tagKey.key);
	if (!resolver) return null;
	return resolver(tagKey, task);
}

export function extractValue<T extends Record<string, unknown>>(tagKey: TagKey, data: T[]): string {
	if (tagKey.index != null) {
		const item = data[tagKey.index];
		if (!item) return '';
		if (tagKey.propertyName) return String(item[tagKey.propertyName] ?? '');
		return String(item);
	}

	if (tagKey.indexPropertyName && tagKey.indexPropertyValue) {
		let item: T | undefined;
		const val = tagKey.indexPropertyValue;

		if (val.endsWith('%')) {
			const prefix = val.slice(0, -1);
			item = data.find((d) => String(d[tagKey.indexPropertyName!] ?? '').startsWith(prefix));
		} else {
			item = data.find((d) => String(d[tagKey.indexPropertyName!] ?? '') === val);
		}

		if (item) {
			if (tagKey.propertyName) return String(item[tagKey.propertyName] ?? '');
			return String(item);
		}
	}

	return '';
}
