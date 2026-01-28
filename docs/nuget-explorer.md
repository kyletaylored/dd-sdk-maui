---
layout: default
title: NuGet Feed
nav_order: 6
---

# NuGet Explorer

This page lists packages published in this repo’s GitHub Pages NuGet feed.

<div class="nuget-explorer">
  <div class="controls">
    <input id="nx-q" class="nx-input" type="search" placeholder="Search packages…" />
    <label class="nx-check">
      <input id="nx-pr" type="checkbox" />
      Show prerelease
    </label>
  </div>

  <div class="meta" id="nx-meta"></div>
  <div class="list" id="nx-list"></div>
</div>

<style>
.nuget-explorer { margin-top: 1rem; }
.nuget-explorer .controls { display:flex; gap: 1rem; align-items:center; flex-wrap: wrap; }
.nuget-explorer .nx-input { min-width: 280px; padding: .55rem .7rem; border: 1px solid #ccc; border-radius: .4rem; }
.nuget-explorer .nx-check { display:flex; gap:.5rem; align-items:center; }
.nuget-explorer .meta { margin: .75rem 0; opacity: .85; }
.nuget-explorer .card {
  border: 1px solid rgba(0,0,0,.12);
  border-radius: .6rem;
  padding: .9rem 1rem;
  margin: .6rem 0;
}
.nuget-explorer .id { font-weight: 700; font-size: 1.05rem; }
.nuget-explorer .row { display:flex; gap: .75rem; flex-wrap: wrap; margin-top:.25rem; opacity:.9; }
.nuget-explorer .pill {
  display:inline-block;
  padding: .15rem .5rem;
  border-radius: 999px;
  border: 1px solid rgba(0,0,0,.15);
  font-size: .85rem;
}
.nuget-explorer .links a { margin-right: .75rem; }
</style>

<script>
(async function () {
  const base = "{{ site.baseurl }}";
  const feedIndexUrl = base + "/nuget/index.json";
  const packagesUrl  = base + "/nuget/packages.json";

  const els = {
    q: document.getElementById("nx-q"),
    pr: document.getElementById("nx-pr"),
    meta: document.getElementById("nx-meta"),
    list: document.getElementById("nx-list"),
  };

  function esc(s) {
    return String(s).replace(/[&<>"']/g, c => ({
      "&":"&amp;","<":"&lt;",">":"&gt;",'"':"&quot;","'":"&#39;"
    }[c]));
  }

  function render(items, totalCount, generatedAtUtc) {
    els.meta.innerHTML =
      `${esc(items.length)} shown (of ${esc(totalCount)}). ` +
      `Index generated: ${esc(generatedAtUtc || "unknown")}. ` +
      `<span class="links">` +
      `<a href="${esc(feedIndexUrl)}">Feed index.json</a>` +
      `<a href="${esc(packagesUrl)}">packages.json</a>` +
      `</span>`;

    if (!items.length) {
      els.list.innerHTML = `<p>No matching packages.</p>`;
      return;
    }

    els.list.innerHTML = items.map(p => {
      const id = p.id;
      const latest = p.latest;
      const latestStable = p.latestStable;
      const hasPre = !!p.hasPrerelease;

      const regUrl = base + "/nuget/registration/" + encodeURIComponent(id) + "/index.json";

      const pills = [
        latestStable ? `<span class="pill">latest stable: ${esc(latestStable)}</span>` : `<span class="pill">no stable</span>`,
        latest ? `<span class="pill">latest: ${esc(latest)}</span>` : `<span class="pill">no versions</span>`,
        hasPre ? `<span class="pill">prerelease</span>` : ""
      ].filter(Boolean).join(" ");

      return `
        <div class="card">
          <div class="id">${esc(id)}</div>
          <div class="row">${pills}</div>
          <div class="row links">
            <a href="${esc(regUrl)}">Registration index</a>
          </div>
        </div>
      `;
    }).join("");
  }

  function applyFilter(all, q, showPrerelease) {
    const qq = (q || "").trim().toLowerCase();
    return all.filter(p => {
      const match = !qq || p.id.toLowerCase().includes(qq);
      if (!match) return false;
      if (showPrerelease) return true;
      // If not showing prerelease, hide packages that ONLY have prerelease versions (no stable)
      return !!p.latestStable;
    });
  }

  let data;
  try {
    const res = await fetch(packagesUrl, { cache: "no-store" });
    if (!res.ok) throw new Error("Failed to load packages.json: " + res.status);
    data = await res.json();
  } catch (e) {
    els.meta.textContent = "Failed to load package index: " + e.message;
    return;
  }

  const all = data.packages || [];
  const totalCount = data.count || all.length;
  const generatedAtUtc = data.generatedAtUtc;

  function update() {
    const items = applyFilter(all, els.q.value, els.pr.checked);
    render(items, totalCount, generatedAtUtc);
  }

  els.q.addEventListener("input", update);
  els.pr.addEventListener("change", update);

  update();
})();
</script>
