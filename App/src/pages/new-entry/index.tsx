import React, { useState, useEffect, useRef } from 'react';
import { createRoot } from 'react-dom/client';
import '../../styles/global.css';
import './new-entry.css';

function NewEntry() {
	const [text, setText] = useState('');
	const [suggestions, setSuggestions] = useState<string[]>([]);
	const [filtered, setFiltered] = useState<string[]>([]);
	const [selectedIdx, setSelectedIdx] = useState(-1);
	const inputRef = useRef<HTMLInputElement>(null);

	useEffect(() => {
		window.zupAPI.on('new-entry:suggestions', (s) => {
			setSuggestions(s as string[]);
			setFiltered(s as string[]);
		});
		setTimeout(() => inputRef.current?.focus(), 100);
	}, []);

	useEffect(() => {
		if (!text.trim()) {
			setFiltered(suggestions);
			return;
		}
		setFiltered(suggestions.filter((s) => s.toLowerCase().includes(text.toLowerCase())));
		setSelectedIdx(-1);
	}, [text, suggestions]);

	const getSelected = (fromTextbox = false): string => {
		if (fromTextbox || selectedIdx < 0) return text.trim();
		return filtered[selectedIdx] ?? text.trim();
	};

	const submit = async (mode: 'stop-others' | 'parallel' | 'queue' | 'blank') => {
		const entry = mode === 'blank' ? '' : getSelected(mode === 'queue' && selectedIdx < 0);
		if (mode !== 'blank' && !entry) return;
		window.close();
		await window.zupAPI.startTask({
			entry,
			startNow: mode !== 'queue',
			stopOtherTask: mode === 'stop-others',
			hideParent: false,
			bringNotes: false,
			bringTags: false,
			getTags: true,
		});
		setText('');
		setSelectedIdx(-1);
	};

	const handleKey = (e: React.KeyboardEvent) => {
		if (e.key === 'Escape') {
			window.close();
			return;
		}
		if (e.ctrlKey && e.key === 'n') {
			e.preventDefault();
			submit('blank');
			return;
		}

		if (e.key === 'Enter') {
			e.preventDefault();
			if (e.altKey) submit('queue');
			else if (e.shiftKey) submit('parallel');
			else submit('stop-others');
			return;
		}

		if (e.key === 'ArrowDown') {
			e.preventDefault();
			setSelectedIdx((i) => Math.min(i + 1, filtered.length - 1));
		}
		if (e.key === 'ArrowUp') {
			e.preventDefault();
			setSelectedIdx((i) => Math.max(i - 1, -1));
		}
	};

	return (
		<div className="container no-drag">
			<div className="header draggable">New Entry</div>
			<div className="body no-drag">
				<input
					ref={inputRef}
					className="text-input"
					value={text}
					onChange={(e) => setText(e.target.value)}
					onKeyDown={handleKey}
					placeholder="Task name…"
					autoFocus
				/>
				{filtered.length > 0 && (
					<ul className="suggestions">
						{filtered.slice(0, 10).map((s, i) => (
							<li
								key={s}
								className={i === selectedIdx ? 'selected' : ''}
								onClick={() => {
									setText(s);
									submit('stop-others');
								}}
								onDoubleClick={() => submit('stop-others')}
							>
								{s}
							</li>
						))}
					</ul>
				)}
				<div className="hints">
					<span>Enter = run &amp; stop others</span>
					<span>Shift+Enter = parallel</span>
					<span>Alt+Enter = queue</span>
					<span>Ctrl+N = blank task</span>
				</div>
			</div>
		</div>
	);
}

createRoot(document.getElementById('root')!).render(<NewEntry />);
