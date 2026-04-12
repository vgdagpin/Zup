// Port of Utility.cs

export interface WeekData {
	weekNumber: number;
	start: string; // ISO date string
	end: string; // ISO date string
	label: string;
}

export function getWeekNumber(date: Date): number {
	const d = new Date(Date.UTC(date.getFullYear(), date.getMonth(), date.getDate()));
	d.setUTCDate(d.getUTCDate() + 4 - (d.getUTCDay() || 7));
	const yearStart = new Date(Date.UTC(d.getUTCFullYear(), 0, 1));
	return Math.ceil(((d.getTime() - yearStart.getTime()) / 86400000 + 1) / 7);
}

export function getWeekData(year: number): WeekData[] {
	const data = new Map<number, WeekData>();
	const from = new Date(year, 0, 1);
	const to = new Date(year, 11, 31);

	for (let d = new Date(from); d <= to; d.setDate(d.getDate() + 1)) {
		const wn = getWeekNumber(d);
		if (data.has(wn)) continue;

		const day = d.getDay(); // 0=Sun, 1=Mon, ...
		const offset = day === 0 ? 0 : -day;
		const weekStart = new Date(d);
		weekStart.setDate(d.getDate() + offset);

		const weekEnd = new Date(weekStart);
		weekEnd.setDate(weekStart.getDate() + 6);
		weekEnd.setHours(23, 59, 59, 999);

		const fmt = (dt: Date) =>
			`${String(dt.getMonth() + 1).padStart(2, '0')}/${String(dt.getDate()).padStart(2, '0')}/${String(dt.getFullYear()).slice(2)}`;
		data.set(wn, {
			weekNumber: wn,
			start: weekStart.toISOString(),
			end: weekEnd.toISOString(),
			label: `${fmt(weekStart)} - ${fmt(weekEnd)}`,
		});
	}

	return Array.from(data.values()).sort((a, b) => a.weekNumber - b.weekNumber);
}

export function getDayShift(date: Date, dayStartStr: string, dayEndStr: string): { start: Date; end: Date } {
	const [startH, startM] = dayStartStr.split(':').map(Number);
	const [endH, endM] = dayEndStr.split(':').map(Number);

	const start = new Date(date);
	start.setHours(startH, startM, 0, 0);

	const end = new Date(date);
	end.setDate(end.getDate() + 1);
	end.setHours(endH, endM, 59, 999);

	return { start, end };
}

export function getDayOfWeek(startedOn: string | null, dayStart: string, dayEnd: string): string | null {
	if (!startedOn) return null;

	const dt = new Date(startedOn);
	const days = ['Sunday', 'Monday', 'Tuesday', 'Wednesday', 'Thursday', 'Friday', 'Saturday'];

	for (let offset = -1; offset <= 1; offset++) {
		const base = new Date(dt);
		base.setDate(base.getDate() + offset);
		base.setHours(0, 0, 0, 0);

		const shift = getDayShift(base, dayStart, dayEnd);
		if (dt >= shift.start && dt <= shift.end) {
			return days[base.getDay()];
		}
	}

	return null;
}
