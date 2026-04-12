import React, { useState, useEffect, useRef } from 'react';
import { createRoot } from 'react-dom/client';
import '../../styles/global.css';
import './floating-button.css';

interface InitData {
	taskId: string;
	taskName: string;
	startedOn: string | null;
}

function formatDuration(startedOn: string | null): string {
	if (!startedOn) return '00:00:00';
	const diff = Math.floor((Date.now() - new Date(startedOn).getTime()) / 1000);
	const h = Math.floor(diff / 3600);
	const m = Math.floor((diff % 3600) / 60);
	const s = diff % 60;
	return `${String(h).padStart(2, '0')}:${String(m).padStart(2, '0')}:${String(s).padStart(2, '0')}`;
}

function FloatingButton() {
	const [data, setData] = useState<InitData | null>(null);
	const [duration, setDuration] = useState('00:00:00');
	const [isRunning, setIsRunning] = useState(true);
	const dragOffset = useRef({ x: 0, y: 0 });
	const isDragging = useRef(false);

	useEffect(() => {
		window.zupAPI.on('floating-button:init', (d) => {
			const init = d as InitData;
			setData(init);
			setIsRunning(true);
		});
		window.zupAPI.on('task:stopped', () => setIsRunning(false));
	}, []);

	useEffect(() => {
		if (!isRunning || !data?.startedOn) return;
		const t = setInterval(() => setDuration(formatDuration(data.startedOn)), 1000);
		setDuration(formatDuration(data.startedOn));
		return () => clearInterval(t);
	}, [isRunning, data]);

	// Attach mousemove/mouseup at the document level so dragging doesn't stop
	// when the cursor moves outside the small pill window bounds.
	useEffect(() => {
		const onMove = (e: MouseEvent) => {
			if (!isDragging.current) return;
			window.zupAPI.moveWindow(e.screenX - dragOffset.current.x, e.screenY - dragOffset.current.y);
		};
		const onUp = () => {
			if (isDragging.current) {
				window.zupAPI.savePosition(window.screenX, window.screenY);
			}
			isDragging.current = false;
		};
		document.addEventListener('mousemove', onMove);
		document.addEventListener('mouseup', onUp);
		return () => {
			document.removeEventListener('mousemove', onMove);
			document.removeEventListener('mouseup', onUp);
		};
	}, []);

	const handleStop = () => {
		if (data) window.zupAPI.stopTask(data.taskId);
	};

	const handleOpen = () => {
		if (data) window.zupAPI.showUpdateEntry(data.taskId);
	};

	const handleMouseDown = (e: React.MouseEvent) => {
		if ((e.target as HTMLElement).closest('.no-drag')) return;
		isDragging.current = true;
		dragOffset.current = { x: e.screenX - window.screenX, y: e.screenY - window.screenY };
		e.preventDefault();
	};

	const label = data ? data.taskName.slice(0, 22) + (data.taskName.length > 22 ? '…' : '') : '—';

	return (
		<div className="pill" onMouseDown={handleMouseDown}>
			<span className="task-name" title={data?.taskName} onDoubleClick={handleOpen}>
				{label}
			</span>
			<span className="duration" onDoubleClick={handleOpen}>
				{duration}
			</span>
			<button className="stop-btn no-drag" onClick={handleStop} title="Stop task">
				■
			</button>
		</div>
	);
}

createRoot(document.getElementById('root')!).render(<FloatingButton />);
