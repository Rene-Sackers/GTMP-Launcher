# A little back story

This project has always been worked on free of charge, for supposed non-profit multiplayer mods. 99% of the code is mine, althought it's fair to mention several people have worked on minor changes since I left the project about a year ago, including: `DurtyFree` (previous owner of GT-MP), `root`, `Msk`, `Jingles` and `Adam` (owner of GTA: Network).

Right, so I started working with [GTA: Network](https://gtanet.work/), which quickly turned into real work with really weird requests like changing the anti-aliasing of a font to make it look different when you zoom in 500%.

GTA: Network "ceased to exist", and [GT-MP](https://gt-mp.net/) was born, with new lead by `DurtyFree`, based on the old code of `GTA: Network`. Which was by the way originally created by `Guad`.

There I worked happily on the launcher until I decided it was ready and functional enough, then I decided to stop working on it proactively, but be available in case there were questions.

About a year later, it turns out `DurtyFree` sold GT-MP for a significant amount of money, including the source code to the launcher that I built free of charge as a community project for a non-profit.

Now I've decided to release the source code to this project as I think it's quite pretty and I'd like to share it with other people.

Not to mention, I was also told the entire project would be open sourced if it reached a stable point. Which I believe it never will.

## Drama aside, here's what it is:

It probably won't build, it's a bit outdated and might use some old packages that are no longer available.

It's a WPF application that queries the masterserver for available servers, updates, etc.

It has a self updater.

It has several tools used to configure the mod.

## Ommitted code

As requested by `Julice`, the current owner of GT-MP, i've ommitted the code that launches and injects the mod into the game. I didn't invent this method either, though I've grossly improved and simplified the original code of it.

URLs and addresses of old/existing services have been removed.