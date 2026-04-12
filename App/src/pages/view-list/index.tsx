import { useState, useEffect, useCallback } from 'react';
import { createRoot } from 'react-dom/client';
import { useReactTable, getCoreRowModel, flexRender, createColumnHelper } from '@tanstack/react-table';
import type { ZupTask, WeekData } from '../../types/index.js';
import '../../styles/global.css';
import './view-list.css';

const col = createColumnHelper<ZupTask>();

const columns = [
	col.accessor('task', { header: 'Task', size: 260 }),
	col.accessor('startedOn', {
		header: 'Started',
		cell: (i) => (i.getValue() ? new Date(i.getValue()!).toLocaleString() : '—'),
		size: 160,
	}),
	col.accessor('endedOn', {
		header: 'Ended',
		cell: (i) => (i.getValue() ? new Date(i.getValue()!).toLocaleString() : '—'),
		size: 160,
	}),
	col.accessor('durationString', { header: 'Duration', cell: (i) => i.getValue() ?? '—', size: 90 }),
	col.accessor('dayOfWeek', { header: 'Day', cell: (i) => i.getValue() ?? '—', size: 80 }),
	col.accessor('tags', { header: 'Tags', cell: (i) => (i.getValue() as string[]).join(', '), size: 120 }),
];

function ViewList() {
	const [tasks, setTasks] = useState<ZupTask[]>([]);
	const [weeks, setWeeks] = useState<WeekData[]>([]);
	const [selectedWeeks, setSelectedWeeks] = useState<number[]>([]);
	const [search, setSearch] = useState('');
	const [settings, setSettings] = useState<{
		exportRowFormat: string;
		exportFileExtension: string;
		timesheetsFolder: string;
	} | null>(null);
	const [exportDate, setExportDate] = useState(new Date().toLocaleDateString('en-US'));
	const [loading, setLoading] = useState(false);

	useEffect(() => {
		const year = new Date().getFullYear();
		Promise.all([window.zupAPI.getWeekData(year), window.zupAPI.getSettings()]).then(([wks, cfg]) => {
			const curWeek = getCurrentWeekNumber();
			const filtered = wks.filter((w) => w.weekNumber <= curWeek).reverse();
			setWeeks(filtered);
			if (filtered.length) setSelectedWeeks([filtered[0].weekNumber]);
			setSettings(cfg);
		});
	}, []);

	const load = useCallback(async () => {
		if (!selectedWeeks.length) return;
		const matching = weeks.filter((w) => selectedWeeks.includes(w.weekNumber));
		const from = matching.map((w) => w.start).sort()[0];
		const to = matching
			.map((w) => w.end)
			.sort()
			.reverse()[0];
		setLoading(true);
		const result = await window.zupAPI.queryTasks(from, to, search || undefined);
		setTasks(result);
		setLoading(false);
	}, [selectedWeeks, weeks, search]);

	useEffect(() => {
		load();
	}, [load]);

	useEffect(() => {
		const refresh = () => load();
		window.zupAPI.on('task:started', refresh);
		window.zupAPI.on('task:stopped', refresh);
		window.zupAPI.on('task:updated', refresh);
		window.zupAPI.on('task:deleted', refresh);
		return () => {
			window.zupAPI.off('task:started', refresh);
			window.zupAPI.off('task:stopped', refresh);
			window.zupAPI.off('task:updated', refresh);
			window.zupAPI.off('task:deleted', refresh);
		};
	}, [load]);

	const table = useReactTable({ data: tasks, columns, getCoreRowModel: getCoreRowModel() });

	const handleExport = async () => {
		if (!settings?.timesheetsFolder || !settings.exportRowFormat) return alert('Configure export settings first');
		await window.zupAPI.exportTimesheets(
			tasks,
			settings.exportRowFormat,
			settings.timesheetsFolder,
			settings.exportFileExtension,
			exportDate,
		);
		window.zupAPI.openPath(settings.timesheetsFolder);
	};

	const toggleWeek = (wn: number, multi: boolean) => {
		if (multi) setSelectedWeeks((prev) => (prev.includes(wn) ? prev.filter((w) => w !== wn) : [...prev, wn]));
		else setSelectedWeeks([wn]);
	};

	return (
		<div className="container">
			<div className="sidebar">
				<div className="sidebar-header">Weeks</div>
				<ul className="week-list">
					{weeks.map((w) => (
						<li
							key={w.weekNumber}
							className={selectedWeeks.includes(w.weekNumber) ? 'selected' : ''}
							onClick={(e) => toggleWeek(w.weekNumber, e.ctrlKey || e.metaKey)}
						>
							{w.label}
						</li>
					))}
				</ul>
			</div>
			<div className="main">
				<div className="toolbar">
					<input className="search" placeholder="Search…" value={search} onChange={(e) => setSearch(e.target.value)} />
					<div style={{ display: 'flex', gap: 6, alignItems: 'center' }}>
						<input
							type="date"
							value={new Date(exportDate).toISOString().slice(0, 10)}
							onChange={(e) => setExportDate(new Date(e.target.value).toLocaleDateString('en-US'))}
							title="Timesheet date"
							style={{ width: 140 }}
						/>
						<button onClick={handleExport}>Export</button>
						<button onClick={() => window.zupAPI.showSettings()}>Settings</button>
					</div>
				</div>
				<div className="table-wrap">
					{loading ? (
						<div className="loading">Loading…</div>
					) : (
						<table>
							<thead>
								{table.getHeaderGroups().map((hg) => (
									<tr key={hg.id}>
										{hg.headers.map((h) => (
											<th key={h.id} style={{ width: h.column.getSize() }}>
												{flexRender(h.column.columnDef.header, h.getContext())}
											</th>
										))}
									</tr>
								))}
							</thead>
							<tbody>
								{table.getRowModel().rows.map((row) => (
									<tr
										key={row.id}
										onDoubleClick={() => window.zupAPI.showUpdateEntry(row.original.id)}
										className={row.original.isRunning ? 'running' : ''}
									>
										{row.getVisibleCells().map((cell) => (
											<td key={cell.id}>{flexRender(cell.column.columnDef.cell, cell.getContext())}</td>
										))}
									</tr>
								))}
							</tbody>
						</table>
					)}
				</div>
				<div className="statusbar">
					{tasks.length} record{tasks.length !== 1 ? 's' : ''}
				</div>
			</div>
		</div>
	);
}

function getCurrentWeekNumber(): number {
	const now = new Date();
	const d = new Date(Date.UTC(now.getFullYear(), now.getMonth(), now.getDate()));
	d.setUTCDate(d.getUTCDate() + 4 - (d.getUTCDay() || 7));
	const y = new Date(Date.UTC(d.getUTCFullYear(), 0, 1));
	return Math.ceil(((d.getTime() - y.getTime()) / 86400000 + 1) / 7);
}

createRoot(document.getElementById('root')!).render(<ViewList />);
