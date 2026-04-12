import { useState, useRef } from 'react';
import './token-input.css';

interface Props {
	value: string[];
	onChange: (tags: string[]) => void;
	suggestions: string[];
	onTokenDoubleClick?: (tag: string) => void;
}

export default function TokenInput({ value, onChange, suggestions, onTokenDoubleClick }: Props) {
	const [input, setInput] = useState('');
	const [showDrop, setShowDrop] = useState(false);
	const inputRef = useRef<HTMLInputElement>(null);

	const filtered = suggestions.filter((s) => s.toLowerCase().includes(input.toLowerCase()) && !value.includes(s)).slice(0, 8);

	const addTag = (tag: string) => {
		const clean = tag.trim();
		if (clean && !value.includes(clean)) onChange([...value, clean]);
		setInput('');
		setShowDrop(false);
		inputRef.current?.focus();
	};

	const removeTag = (tag: string) => onChange(value.filter((t) => t !== tag));

	const handleKey = (e: React.KeyboardEvent) => {
		if ((e.key === 'Enter' || e.key === ',') && input.trim()) {
			e.preventDefault();
			addTag(input);
		}
		if (e.key === 'Backspace' && !input && value.length) {
			onChange(value.slice(0, -1));
		}
		if (e.key === 'Escape') setShowDrop(false);
	};

	return (
		<div className="token-input-wrapper" onClick={() => inputRef.current?.focus()}>
			{value.map((tag) => (
				<span key={tag} className="token" onDoubleClick={() => onTokenDoubleClick?.(tag)}>
					{tag}
					<button
						onClick={(e) => {
							e.stopPropagation();
							removeTag(tag);
						}}
					>
						×
					</button>
				</span>
			))}
			<input
				ref={inputRef}
				value={input}
				onChange={(e) => {
					setInput(e.target.value);
					setShowDrop(true);
				}}
				onKeyDown={handleKey}
				onFocus={() => setShowDrop(true)}
				onBlur={() => setTimeout(() => setShowDrop(false), 150)}
				placeholder={value.length === 0 ? 'Add tags…' : ''}
				className="token-input"
			/>
			{showDrop && filtered.length > 0 && (
				<ul className="token-dropdown">
					{filtered.map((s) => (
						<li key={s} onMouseDown={() => addTag(s)}>
							{s}
						</li>
					))}
				</ul>
			)}
		</div>
	);
}
