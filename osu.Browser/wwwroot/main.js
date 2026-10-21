// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

import { dotnet } from './_framework/dotnet.js';

const overlay = document.getElementById('overlay');
const status = document.getElementById('status');
const progressBar = document.getElementById('progress-bar');
const start = document.getElementById('start');

const runtime = await dotnet
    .withConfig({
        onDownloadResourceProgress: (loaded, total) => {
            progressBar.style.width = `${Math.round(loaded / total * 100)}%`;
        },
    })
    .create();

// Browsers only allow audio playback after the user has interacted with the page, so wait for that before starting.
status.textContent = 'Ready!';
start.hidden = false;

await new Promise(resolve => start.addEventListener('click', resolve, { once: true }));

overlay.hidden = true;
document.getElementById('canvas').focus();

// Note that the runtime must not be exited after Main() returns, as the game keeps running via the browser's event loop.
await runtime.runMain();
