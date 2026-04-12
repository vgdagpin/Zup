export type TaskStatus = 'Running' | 'Ranked' | 'Queued' | 'Unclosed' | 'Closed' | 'Ongoing';

export interface ZupTask {
	id: string;
	task: string;
	createdOn: string;
	startedOn: string | null;
	endedOn: string | null;
	reminder: string | null;
	rank: number | null;
	isRunning: boolean;
	tags: string[];
	duration?: number | null; // seconds
	durationString?: string | null;
	dayOfWeek?: string | null;
}

export interface ZupTag {
	id: string;
	name: string;
	description: string | null;
}

export interface ZupNote {
	id: string;
	taskId: string;
	notes: string;
	rtf: string | null;
	createdOn: string;
	updatedOn: string | null;
	summary: string;
}

export interface ZupSettings {
	showQueuedTasks: boolean;
	showRankedTasks: boolean;
	showClosedTasks: boolean;
	entryListOpacity: number;
	numDaysOfDataToLoad: number;
	itemsToShow: number;
	autoOpenUpdateWindow: boolean;
	usePillTimer: boolean;
	dayStart: string; // "HH:mm"
	dayEnd: string; // "HH:mm"
	dayEndNextDay: boolean;
	timesheetsFolder: string;
	exportRowFormat: string;
	exportFileExtension: string;
	trimDaysToKeep: number;
	formLocationX: number;
	formLocationY: number;
	dbPath: string;
}

export interface NewEntryArgs {
	entry: string;
	startNow: boolean;
	stopOtherTask: boolean;
	hideParent: boolean;
	bringNotes: boolean;
	bringTags: boolean;
	getTags: boolean;
	parentEntryId?: string | null;
}

export interface UpdateEntryArgs {
	task: string;
	startedOn: string | null;
	endedOn: string | null;
	rank: number | null;
	tags: string[];
}

export interface WeekData {
	weekNumber: number;
	start: string;
	end: string;
	label: string;
}

export function getTaskStatus(task: Pick<ZupTask, 'isRunning' | 'rank' | 'startedOn' | 'endedOn'>): TaskStatus {
	if (task.isRunning) return 'Running';
	if (task.rank != null) return 'Ranked';
	if (task.startedOn == null) return 'Queued';
	if (task.startedOn != null && task.endedOn == null) return 'Unclosed';
	if (task.startedOn != null && task.endedOn != null) return 'Closed';
	return 'Ongoing';
}
