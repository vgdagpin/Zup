import { defineConfig } from 'vite';
import path from 'node:path';
import electron from 'vite-plugin-electron/simple';
import react from '@vitejs/plugin-react';

const pages = ['entry-list', 'floating-button', 'new-entry', 'update-entry', 'view-list', 'settings', 'tag-editor'];

export default defineConfig({
	plugins: [
		react(),
		electron({
			main: {
				entry: 'electron/main.ts',
				vite: {
					build: {
						rollupOptions: {
							external: ['better-sqlite3'],
						},
					},
				},
			},
			preload: {
				input: path.join(__dirname, 'electron/preload.ts'),
			},
			renderer: process.env.NODE_ENV === 'test' ? undefined : {},
		}),
	],
	build: {
		rollupOptions: {
			input: Object.fromEntries(pages.map((p) => [p, path.join(__dirname, `${p}.html`)])),
		},
	},
});
