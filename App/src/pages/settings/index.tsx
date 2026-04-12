import { useState, useEffect } from 'react';
import { createRoot } from 'react-dom/client';
import type { ZupSettings } from '../../types/index.js';
import '../../styles/global.css';
import './settings.css';

function Settings() {
	const [s, setS] = useState<ZupSettings | null>(null);
	const [saved, setSaved] = useState(false);

	useEffect(() => {
		window.zupAPI.getSettings().then(setS);
	}, []);

	const set = <K extends keyof ZupSettings>(key: K, value: ZupSettings[K]) => {
		setS((prev) => (prev ? { ...prev, [key]: value } : prev));
		setSaved(false);
	};

	const save = async () => {
		if (!s) return;
		for (const [k, v] of Object.entries(s)) await window.zupAPI.setSetting(k, v);
		setSaved(true);
		setTimeout(() => setSaved(false), 2000);
	};

	const handleDbChange = async () => {
		// Trigger native file dialog via showOpenDialog IPC — simplified here
		const newPath = prompt('Enter new database path:') ?? '';
		if (!newPath) return;
		await window.zupAPI.changeDbPath(newPath);
		setS((prev) => (prev ? { ...prev, dbPath: newPath } : prev));
	};

	const backup = async () => {
		const dir = await window.zupAPI.backupDb();
		alert(dir ? `Backup saved to: ${dir}` : 'Backup failed');
	};

	const trim = async () => {
		if (!s) return;
		if (!confirm(`Delete tasks older than ${s.trimDaysToKeep} days?`)) return;
		const n = await window.zupAPI.trimTasks(s.trimDaysToKeep);
		alert(`Trimmed ${n} record(s)`);
	};

	if (!s) return <div className="loading">Loading…</div>;

	return (
		<div className="container">
			<div className="header">Settings</div>
			<div className="content">
				<Section title="Display">
					<Field label="Items to show">
						<input
							type="number"
							min="1"
							max="20"
							value={s.itemsToShow}
							onChange={(e) => set('itemsToShow', parseInt(e.target.value))}
						/>
					</Field>
					<Field label="Opacity">
						<input
							type="range"
							min="20"
							max="100"
							value={Math.round(s.entryListOpacity * 100)}
							onChange={(e) => set('entryListOpacity', parseInt(e.target.value) / 100)}
						/>
						<span>{Math.round(s.entryListOpacity * 100)}%</span>
					</Field>
					<Field label="Days to load">
						<input
							type="number"
							min="1"
							value={s.numDaysOfDataToLoad}
							onChange={(e) => set('numDaysOfDataToLoad', parseInt(e.target.value))}
						/>
					</Field>
					<Field label="Show queued tasks">
						<input
							type="checkbox"
							checked={s.showQueuedTasks}
							onChange={(e) => set('showQueuedTasks', e.target.checked)}
						/>
					</Field>
					<Field label="Show ranked tasks">
						<input
							type="checkbox"
							checked={s.showRankedTasks}
							onChange={(e) => set('showRankedTasks', e.target.checked)}
						/>
					</Field>
				</Section>

				<Section title="Behaviour">
					<Field label="Use pill timer">
						<input type="checkbox" checked={s.usePillTimer} onChange={(e) => set('usePillTimer', e.target.checked)} />
					</Field>
					<Field label="Auto-open update window">
						<input
							type="checkbox"
							checked={s.autoOpenUpdateWindow}
							onChange={(e) => set('autoOpenUpdateWindow', e.target.checked)}
						/>
					</Field>
				</Section>

				<Section title="Work Hours">
					<Field label="Day start (HH:MM)">
						<input type="time" value={s.dayStart} onChange={(e) => set('dayStart', e.target.value)} />
					</Field>
					<Field label="Day end (HH:MM)">
						<input type="time" value={s.dayEnd} onChange={(e) => set('dayEnd', e.target.value)} />
					</Field>
					<Field label="Day end is next day">
						<input
							type="checkbox"
							checked={s.dayEndNextDay}
							onChange={(e) => set('dayEndNextDay', e.target.checked)}
						/>
					</Field>
				</Section>

				<Section title="Export">
					<Field label="Timesheets folder">
						<input
							value={s.timesheetsFolder}
							onChange={(e) => set('timesheetsFolder', e.target.value)}
							style={{ flex: 1 }}
						/>
					</Field>
					<Field label="Row format">
						<input
							value={s.exportRowFormat}
							onChange={(e) => set('exportRowFormat', e.target.value)}
							style={{ flex: 1 }}
						/>
					</Field>
					<Field label="File extension">
						<input
							value={s.exportFileExtension}
							onChange={(e) => set('exportFileExtension', e.target.value)}
							style={{ width: 80 }}
						/>
					</Field>
				</Section>

				<Section title="Database">
					<Field label="Database path">
						<input value={s.dbPath} readOnly style={{ flex: 1 }} />
						<button onClick={handleDbChange}>Change…</button>
					</Field>
					<Field label="">
						<button onClick={backup}>Backup Now</button>
						<button onClick={trim}>Trim ({s.trimDaysToKeep} days to keep)</button>
					</Field>
					<Field label="Days to keep on trim">
						<input
							type="number"
							min="1"
							value={s.trimDaysToKeep}
							onChange={(e) => set('trimDaysToKeep', parseInt(e.target.value))}
						/>
					</Field>
				</Section>
			</div>
			<div className="footer">
				<button className="primary" onClick={save}>
					{saved ? '✓ Saved' : 'Save'}
				</button>
			</div>
		</div>
	);
}

function Section({ title, children }: { title: string; children: React.ReactNode }) {
	return (
		<div className="section">
			<div className="section-title">{title}</div>
			{children}
		</div>
	);
}

function Field({ label, children }: { label: string; children: React.ReactNode }) {
	return (
		<div className="field">
			<label>{label}</label>
			<div className="field-content">{children}</div>
		</div>
	);
}

createRoot(document.getElementById('root')!).render(<Settings />);
