import { useState, useEffect, useCallback } from 'react';
import { createRoot } from 'react-dom/client';
import type { ZupTask } from '../../types/index.js';
import { getTaskStatus } from '../../types/index.js';
import '../../styles/global.css';
import './entry-list.css';

type TaskGroup = 'running' | 'stopped' | 'queued' | 'ranked';

function groupTask(t: ZupTask): TaskGroup {
	const s = getTaskStatus(t);
	if (s === 'Running') return 'running';
	if (s === 'Queued') return 'queued';
	if (s === 'Ranked') return 'ranked';
	return 'stopped';
}

function EntryList() {
	const [tasks, setTasks] = useState<ZupTask[]>([]);
	const [settings, setSettings] = useState({ itemsToShow: 5, opacity: 1 });
	const [deleteTarget, setDeleteTarget] = useState<string | null>(null);

	const load = useCallback(async () => {
		const [ts, cfg] = await Promise.all([window.zupAPI.getTasks(), window.zupAPI.getSettings()]);
		setTasks(ts);
		setSettings({ itemsToShow: cfg.itemsToShow, opacity: cfg.entryListOpacity });
	}, []);

	useEffect(() => {
		load();
		const refresh = () => load();
		window.zupAPI.on('task:started', refresh);
		window.zupAPI.on('task:stopped', refresh);
		window.zupAPI.on('task:deleted', refresh);
		window.zupAPI.on('task:updated', refresh);
		return () => {
			window.zupAPI.off('task:started', refresh);
			window.zupAPI.off('task:stopped', refresh);
			window.zupAPI.off('task:deleted', refresh);
			window.zupAPI.off('task:updated', refresh);
		};
	}, [load]);

	const running = tasks.filter((t) => groupTask(t) === 'running').slice(0, 1);
	const stopped = tasks.filter((t) => groupTask(t) === 'stopped').slice(0, settings.itemsToShow);
	const queued = tasks.filter((t) => groupTask(t) === 'queued').slice(0, 2);
	const ranked = tasks.filter((t) => groupTask(t) === 'ranked').slice(0, 4);

	const handleStop = (t: ZupTask) => window.zupAPI.stopTask(t.id);
	const handleStart = async (t: ZupTask) => {
		await window.zupAPI.startTask({
			entry: t.task,
			startNow: true,
			stopOtherTask: true,
			hideParent: false,
			bringNotes: false,
			bringTags: false,
			getTags: true,
			parentEntryId: t.id,
		});
	};
	const handleEdit = (t: ZupTask) => window.zupAPI.showUpdateEntry(t.id);
	const handleDelete = async (id: string) => {
		await window.zupAPI.deleteTask(id);
		setDeleteTarget(null);
	};

	const renderTask = (t: ZupTask, group: TaskGroup) => (
		<div
			key={t.id}
			className={`task-row no-drag ${group}`}
			onDoubleClick={() => handleEdit(t)}
			onContextMenu={(e) => {
				e.preventDefault();
				setDeleteTarget(t.id);
			}}
		>
			<button className="toggle-btn" onClick={() => toggleTask(t)} title={t.isRunning ? 'Stop' : 'Start'}>
				{t.isRunning ? '■' : '▶'}
			</button>
			<span className="task-text" title={t.task}>
				{t.task}
			</span>
			{t.tags.length > 0 && <span className="tags">{t.tags.slice(0, 2).join(', ')}</span>}
		</div>
	);

	const toggleTask = (t: ZupTask) => {
		if (t.isRunning) handleStop(t);
		else handleStart(t);
	};

	return (
		<div className="container draggable" style={{ opacity: settings.opacity }}>
			{/* Title bar */}
			<div className="titlebar">
				<span className="title">Zup</span>
				<div className="titlebar-actions no-drag">
					<button onClick={() => window.zupAPI.showNewEntry()} title="New Entry (Shift+Alt+J)">
						+
					</button>
					<button onClick={() => window.zupAPI.showViewList()} title="View All">
						⊞
					</button>
					<button onClick={() => window.zupAPI.moveToCenter()} title="Move to center">
						⊡
					</button>
				</div>
			</div>

			{/* Running */}
			{running.map((t) => renderTask(t, 'running'))}

			{/* Separator */}
			{stopped.length > 0 && <div className="separator" />}

			{/* Stopped (previous) */}
			{stopped.map((t) => renderTask(t, 'stopped'))}

			{/* Queued */}
			{queued.length > 0 && (
				<div className="separator queued-sep">
					<span>Queued</span>
				</div>
			)}
			{queued.map((t) => renderTask(t, 'queued'))}

			{/* Ranked */}
			{ranked.length > 0 && (
				<div className="separator ranked-sep">
					<span>Ranked</span>
				</div>
			)}
			{ranked.map((t) => renderTask(t, 'ranked'))}

			{/* Delete confirmation */}
			{deleteTarget && (
				<div className="confirm-overlay no-drag">
					<p>Delete this task?</p>
					<div className="confirm-btns">
						<button className="danger" onClick={() => handleDelete(deleteTarget)}>
							Delete
						</button>
						<button onClick={() => setDeleteTarget(null)}>Cancel</button>
					</div>
				</div>
			)}
		</div>
	);
}

createRoot(document.getElementById('root')!).render(<EntryList />);
