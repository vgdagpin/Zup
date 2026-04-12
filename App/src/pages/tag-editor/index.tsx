import { useState, useEffect } from 'react';
import { createRoot } from 'react-dom/client';
import type { ZupTag } from '../../types/index.js';
import '../../styles/global.css';
import './tag-editor.css';

function TagEditor() {
	const [tags, setTags] = useState<ZupTag[]>([]);
	const [selected, setSelected] = useState<ZupTag | null>(null);
	const [name, setName] = useState('');
	const [description, setDescription] = useState('');

	const load = async () => {
		const ts = await window.zupAPI.getTags();
		setTags(ts);
	};

	useEffect(() => {
		load();
		window.zupAPI.on('tag-editor:select', (tagName) => {
			const tag = tags.find((t) => t.name === tagName);
			if (tag) selectTag(tag);
		});
	}, []);

	const selectTag = (tag: ZupTag) => {
		setSelected(tag);
		setName(tag.name);
		setDescription(tag.description ?? '');
	};

	const save = async () => {
		if (!selected) return;
		await window.zupAPI.updateTag(selected.id, name, description);
		await load();
		alert('Tag updated');
	};

	const del = async () => {
		if (!selected) return;
		if (!confirm('Delete this tag?')) return;
		const ok = await window.zupAPI.deleteTag(selected.id);
		if (!ok) {
			alert('Tag is in use and cannot be deleted');
			return;
		}
		await load();
		setSelected(null);
		setName('');
		setDescription('');
	};

	return (
		<div className="container">
			<div className="header">Tag Editor</div>
			<div className="body">
				<div className="list-panel">
					<ul>
						{tags.map((t) => (
							<li key={t.id} className={selected?.id === t.id ? 'active' : ''} onClick={() => selectTag(t)}>
								{t.name}
							</li>
						))}
					</ul>
				</div>
				<div className="detail-panel">
					<div className="field">
						<label>Name</label>
						<input value={name} onChange={(e) => setName(e.target.value)} disabled={!selected} />
					</div>
					<div className="field">
						<label>Description</label>
						<textarea
							value={description}
							onChange={(e) => setDescription(e.target.value)}
							disabled={!selected}
							rows={4}
						/>
					</div>
					<div className="actions">
						<button className="primary" onClick={save} disabled={!selected}>
							Save
						</button>
						<button className="danger" onClick={del} disabled={!selected}>
							Delete
						</button>
					</div>
				</div>
			</div>
		</div>
	);
}

createRoot(document.getElementById('root')!).render(<TagEditor />);
