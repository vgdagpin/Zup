import { useState, useEffect, useCallback } from 'react';
import { createRoot } from 'react-dom/client';
import { useEditor, EditorContent } from '@tiptap/react';
import StarterKit from '@tiptap/starter-kit';
import Link from '@tiptap/extension-link';
import type { ZupTask, ZupNote } from '../../types/index.js';
import { getTaskStatus } from '../../types/index.js';
import TokenInput from '../../components/TokenInput/index.js';
import '../../styles/global.css';
import './update-entry.css';

function UpdateEntry() {
	const [task, setTask] = useState<ZupTask | null>(null);
	const [taskText, setTaskText] = useState('');
	const [startedOn, setStartedOn] = useState('');
	const [endedOn, setEndedOn] = useState('');
	const [rank, setRank] = useState(0);
	const [tags, setTags] = useState<string[]>([]);
	const [notes, setNotes] = useState<ZupNote[]>([]);
	const [selectedNoteId, setSelectedNoteId] = useState<string | null>(null);
	const [allTags, setAllTags] = useState<string[]>([]);
	const [modified, setModified] = useState(false);

	const editor = useEditor({
		extensions: [StarterKit, Link.configure({ openOnClick: false })],
		content: '',
		onUpdate: () => setModified(true),
	});

	const loadTask = useCallback(async (id: string) => {
		const [all, ts] = await Promise.all([window.zupAPI.getTags(), window.zupAPI.getTasks()]);
		setAllTags(all.map((t) => t.name));
		const found = ts.find((t) => t.id === id);
		if (!found) return;
		setTask(found);
		setTaskText(found.task);
		setStartedOn(found.startedOn ?? '');
		setEndedOn(found.endedOn ?? '');
		setRank(found.rank ?? 0);
		setTags(found.tags);
		setModified(false);
		loadNotes(id);
	}, []);

	const loadNotes = async (taskId: string) => {
		const ns = await window.zupAPI.getNotes(taskId);
		setNotes(ns);
		setSelectedNoteId(null);
		editor?.commands.setContent('');
	};

	useEffect(() => {
		window.zupAPI.on('update-entry:load', (id) => loadTask(id as string));
		window.zupAPI.on('task:stopped', () => {
			if (task) loadTask(task.id);
		});
		window.zupAPI.on('task:updated', (t) => {
			const updated = t as ZupTask;
			if (task && updated.id === task.id) {
				setStartedOn(updated.startedOn ?? '');
				setEndedOn(updated.endedOn ?? '');
			}
		});
	}, [task, loadTask]);

	const selectNote = (note: ZupNote) => {
		setSelectedNoteId(note.id);
		editor?.commands.setContent(note.rtf ?? note.notes);
	};

	const saveEntry = async () => {
		if (!task) return;
		await window.zupAPI.updateTask(task.id, {
			task: taskText,
			startedOn: startedOn || null,
			endedOn: endedOn || null,
			rank: rank > 0 ? rank : null,
			tags,
		});
		setModified(false);
	};

	const saveNote = async () => {
		if (!task || !editor) return;
		const html = editor.getHTML();
		const plain = editor.getText();
		await window.zupAPI.saveNote(task.id, selectedNoteId, plain, html);
		loadNotes(task.id);
		setModified(false);
	};

	const deleteNote = async () => {
		if (!selectedNoteId || !task) return;
		await window.zupAPI.deleteNote(selectedNoteId);
		loadNotes(task.id);
	};

	const rerun = async () => {
		if (!task) return;
		const status = getTaskStatus(task);
		if (status === 'Running') {
			await window.zupAPI.stopTask(task.id);
		} else if (status === 'Closed' || status === 'Unclosed') {
			await window.zupAPI.startTask({
				entry: task.task,
				startNow: true,
				stopOtherTask: true,
				hideParent: false,
				bringNotes: false,
				bringTags: true,
				getTags: false,
				parentEntryId: task.id,
			});
		}
	};

	const titleSuffix = modified ? ' ●' : '';

	return (
		<div className="container">
			<div className="toolbar">
				<span className="win-title">Update Entry{titleSuffix}</span>
				<div className="toolbar-actions">
					<button onClick={saveEntry} className="primary" disabled={!task}>
						Save
					</button>
					<button onClick={rerun} disabled={!task}>
						{task && getTaskStatus(task) === 'Running' ? 'Stop' : 'Run'}
					</button>
				</div>
			</div>

			<div className="form">
				<div className="field">
					<label>Task</label>
					<input
						value={taskText}
						onChange={(e) => {
							setTaskText(e.target.value);
							setModified(true);
						}}
					/>
				</div>
				<div className="row">
					<div className="field">
						<label>Started On</label>
						<input
							type="datetime-local"
							value={startedOn ? new Date(startedOn).toISOString().slice(0, 16) : ''}
							onChange={(e) => {
								setStartedOn(e.target.value ? new Date(e.target.value).toISOString() : '');
								setModified(true);
							}}
						/>
					</div>
					<div className="field">
						<label>Ended On</label>
						<input
							type="datetime-local"
							value={endedOn ? new Date(endedOn).toISOString().slice(0, 16) : ''}
							onChange={(e) => {
								setEndedOn(e.target.value ? new Date(e.target.value).toISOString() : '');
								setModified(true);
							}}
						/>
					</div>
					<div className="field rank-field">
						<label>Rank</label>
						<input
							type="number"
							min="0"
							max="255"
							value={rank}
							onChange={(e) => {
								setRank(parseInt(e.target.value) || 0);
								setModified(true);
							}}
						/>
					</div>
				</div>
				<div className="field">
					<label>Tags</label>
					<TokenInput
						value={tags}
						onChange={(t) => {
							setTags(t);
							setModified(true);
						}}
						suggestions={allTags}
						onTokenDoubleClick={(tag) => window.zupAPI.showTagEditor(tag)}
					/>
				</div>
			</div>

			<div className="notes-section">
				<div className="notes-list">
					<div className="notes-header">
						<span>Notes</span>
						<button
							onClick={() => {
								setSelectedNoteId(null);
								editor?.commands.setContent('');
							}}
						>
							+ New
						</button>
					</div>
					{notes.map((n) => (
						<div
							key={n.id}
							className={`note-item ${selectedNoteId === n.id ? 'active' : ''}`}
							onClick={() => selectNote(n)}
						>
							{n.summary || '(empty)'}
						</div>
					))}
				</div>
				<div className="notes-editor">
					<div className="editor-toolbar">
						<button onClick={() => editor?.chain().focus().toggleBold().run()}>B</button>
						<button onClick={() => editor?.chain().focus().toggleItalic().run()}>
							<i>I</i>
						</button>
						<button onClick={() => editor?.chain().focus().toggleBulletList().run()}>•</button>
					</div>
					<EditorContent editor={editor} className="tiptap-editor" />
					<div className="notes-actions">
						<button onClick={saveNote} className="primary">
							Save Note
						</button>
						{selectedNoteId && (
							<button onClick={deleteNote} className="danger">
								Delete
							</button>
						)}
					</div>
				</div>
			</div>
		</div>
	);
}

createRoot(document.getElementById('root')!).render(<UpdateEntry />);
