# Interactive Demos

Live, playable demonstrations of core mechanics from the toolkit. Click/drag to interact.

---

## Easing Curves

All 31 easing functions visualized. Select a curve to see it animate in real-time.

<div id="easing-demo" style="background:#1a1a2e;border-radius:8px;padding:1rem;margin:1rem 0">
<canvas id="easing-canvas" width="760" height="300" style="width:100%;border-radius:4px;cursor:pointer"></canvas>
<div id="easing-btns" style="display:flex;flex-wrap:wrap;gap:4px;margin-top:8px"></div>
</div>

<script>
(function(){
  const canvas = document.getElementById('easing-canvas');
  const ctx = canvas.getContext('2d');
  const btnContainer = document.getElementById('easing-btns');

  const ease = {
    Linear: t => t,
    QuadIn: t => t*t,
    QuadOut: t => t*(2-t),
    QuadInOut: t => t<.5?2*t*t:-1+(4-2*t)*t,
    CubicIn: t => t*t*t,
    CubicOut: t => (--t)*t*t+1,
    CubicInOut: t => t<.5?4*t*t*t:(t-1)*(2*t-2)*(2*t-2)+1,
    SineIn: t => 1-Math.cos(t*Math.PI/2),
    SineOut: t => Math.sin(t*Math.PI/2),
    SineInOut: t => -(Math.cos(Math.PI*t)-1)/2,
    ExpoIn: t => t===0?0:Math.pow(2,10*(t-1)),
    ExpoOut: t => t===1?1:1-Math.pow(2,-10*t),
    CircIn: t => 1-Math.sqrt(1-t*t),
    CircOut: t => Math.sqrt(1-(--t)*t),
    BackIn: t => 2.70158*t*t*t-1.70158*t*t,
    BackOut: t => {const c=1.70158;return 1+(c+1)*Math.pow(t-1,3)+c*Math.pow(t-1,2);},
    ElasticOut: t => t===0?0:t===1?1:Math.pow(2,-10*t)*Math.sin((t-0.075)*(2*Math.PI)/0.3)+1,
    BounceOut: t => {if(t<1/2.75)return 7.5625*t*t;if(t<2/2.75)return 7.5625*(t-=1.5/2.75)*t+.75;if(t<2.5/2.75)return 7.5625*(t-=2.25/2.75)*t+.9375;return 7.5625*(t-=2.625/2.75)*t+.984375;},
  };

  let current = 'QuadOut';
  let anim = 0;
  let animDir = 1;

  function createButtons(){
    Object.keys(ease).forEach(name => {
      const btn = document.createElement('button');
      btn.textContent = name;
      btn.style.cssText = 'padding:2px 8px;font-size:11px;border:1px solid #444;background:'+(name===current?'#ff6a35':'#2a2a3e')+';color:#eee;border-radius:3px;cursor:pointer;font-family:monospace';
      btn.onclick = () => { current = name; anim = 0; animDir = 1; createButtons(); };
      btnContainer.appendChild(btn);
    });
  }

  function draw(){
    ctx.fillStyle = '#1a1a2e';
    ctx.fillRect(0,0,canvas.width,canvas.height);

    const pad = 40, w = canvas.width-pad*2, h = canvas.height-pad*2;
    const fn = ease[current];

    // Grid
    ctx.strokeStyle = '#333';
    ctx.lineWidth = 1;
    for(let i=0;i<=10;i++){
      const x = pad+w*i/10, y = pad+h*i/10;
      ctx.beginPath();ctx.moveTo(x,pad);ctx.lineTo(x,pad+h);ctx.stroke();
      ctx.beginPath();ctx.moveTo(pad,y);ctx.lineTo(pad+w,y);ctx.stroke();
    }

    // Curve
    ctx.strokeStyle = '#ff6a35';
    ctx.lineWidth = 3;
    ctx.beginPath();
    for(let i=0;i<=200;i++){
      const t=i/200, x=pad+t*w, y=pad+h-fn(t)*h;
      i===0?ctx.moveTo(x,y):ctx.lineTo(x,y);
    }
    ctx.stroke();

    // Animated ball
    const ballT = fn(anim);
    const bx = pad+anim*w, by = pad+h-ballT*h;
    ctx.fillStyle = '#ff6a35';
    ctx.beginPath();ctx.arc(bx,by,8,0,Math.PI*2);ctx.fill();
    ctx.fillStyle = '#ff6a3544';
    ctx.beginPath();ctx.arc(bx,by,14,0,Math.PI*2);ctx.fill();

    // Labels
    ctx.fillStyle = '#888';
    ctx.font = '12px monospace';
    ctx.fillText('0', pad-15, pad+h+4);
    ctx.fillText('1', pad+w+5, pad+h+4);
    ctx.fillText('1', pad-15, pad+4);
    ctx.fillStyle = '#ff6a35';
    ctx.font = 'bold 14px monospace';
    ctx.fillText(current, pad, pad-10);

    // Animate
    anim += 0.008 * animDir;
    if(anim > 1){anim=1;animDir=-1;}
    if(anim < 0){anim=0;animDir=1;}

    requestAnimationFrame(draw);
  }

  btnContainer.innerHTML = '';
  createButtons();
  draw();
})();
</script>

---

## A* Pathfinding

Click to place walls, right-click to set start/end. Watch A* find the shortest path.

<div id="pathfinding-demo" style="background:#1a1a2e;border-radius:8px;padding:1rem;margin:1rem 0">
<canvas id="path-canvas" width="760" height="400" style="width:100%;border-radius:4px;cursor:crosshair"></canvas>
<p style="color:#888;font-size:12px;margin:8px 0 0">Left click: toggle wall · Right click: set start (green) then end (red) · Space: find path</p>
</div>

<script>
(function(){
  const canvas = document.getElementById('path-canvas');
  const ctx = canvas.getContext('2d');
  const COLS=38, ROWS=20, SIZE=20;
  const grid = Array.from({length:ROWS},()=>Array(COLS).fill(0));
  let start={x:1,y:1}, end={x:COLS-2,y:ROWS-2};
  let path=[], visited=[], settingStart=true;

  function heuristic(a,b){return Math.abs(a.x-b.x)+Math.abs(a.y-b.y);}

  function astar(){
    const open=[{...start,g:0,f:heuristic(start,end),parent:null}];
    const closed=new Set();
    visited=[];path=[];

    while(open.length){
      open.sort((a,b)=>a.f-b.f);
      const cur=open.shift();
      const key=cur.x+','+cur.y;
      if(closed.has(key))continue;
      closed.add(key);
      visited.push({x:cur.x,y:cur.y});

      if(cur.x===end.x&&cur.y===end.y){
        let n=cur;while(n){path.unshift({x:n.x,y:n.y});n=n.parent;}
        return;
      }

      for(const[dx,dy]of[[0,-1],[1,0],[0,1],[-1,0]]){
        const nx=cur.x+dx,ny=cur.y+dy;
        if(nx<0||ny<0||nx>=COLS||ny>=ROWS||grid[ny][nx]||closed.has(nx+','+ny))continue;
        const g=cur.g+1;
        open.push({x:nx,y:ny,g,f:g+heuristic({x:nx,y:ny},end),parent:cur});
      }
    }
  }

  function draw(){
    ctx.fillStyle='#1a1a2e';ctx.fillRect(0,0,canvas.width,canvas.height);
    for(let y=0;y<ROWS;y++)for(let x=0;x<COLS;x++){
      const px=x*SIZE,py=y*SIZE;
      if(grid[y][x]){ctx.fillStyle='#444';ctx.fillRect(px+1,py+1,SIZE-2,SIZE-2);}
      else{ctx.strokeStyle='#2a2a3e';ctx.strokeRect(px,py,SIZE,SIZE);}
    }
    visited.forEach(v=>{ctx.fillStyle='#ff6a3522';ctx.fillRect(v.x*SIZE+1,v.y*SIZE+1,SIZE-2,SIZE-2);});
    path.forEach(p=>{ctx.fillStyle='#ff6a35';ctx.fillRect(p.x*SIZE+2,p.y*SIZE+2,SIZE-4,SIZE-4);});
    ctx.fillStyle='#4ade80';ctx.fillRect(start.x*SIZE+2,start.y*SIZE+2,SIZE-4,SIZE-4);
    ctx.fillStyle='#f87171';ctx.fillRect(end.x*SIZE+2,end.y*SIZE+2,SIZE-4,SIZE-4);
  }

  canvas.addEventListener('click',e=>{
    const r=canvas.getBoundingClientRect();
    const x=Math.floor((e.clientX-r.left)/(r.width/COLS));
    const y=Math.floor((e.clientY-r.top)/(r.height/ROWS));
    if(x>=0&&y>=0&&x<COLS&&y<ROWS){
      grid[y][x]=grid[y][x]?0:1;
      path=[];visited=[];draw();
    }
  });

  canvas.addEventListener('contextmenu',e=>{
    e.preventDefault();
    const r=canvas.getBoundingClientRect();
    const x=Math.floor((e.clientX-r.left)/(r.width/COLS));
    const y=Math.floor((e.clientY-r.top)/(r.height/ROWS));
    if(x>=0&&y>=0&&x<COLS&&y<ROWS&&!grid[y][x]){
      if(settingStart){start={x,y};}else{end={x,y};}
      settingStart=!settingStart;
      path=[];visited=[];draw();
    }
  });

  document.addEventListener('keydown',e=>{
    if(e.code==='Space'){e.preventDefault();astar();draw();}
  });

  // Auto-generate some walls
  for(let i=0;i<80;i++){
    const x=Math.floor(Math.random()*COLS),y=Math.floor(Math.random()*ROWS);
    if((x!==start.x||y!==start.y)&&(x!==end.x||y!==end.y))grid[y][x]=1;
  }
  draw();
})();
</script>

---

## Particle System

A simple particle emitter. Move your mouse to control the emission point.

<div id="particle-demo" style="background:#1a1a2e;border-radius:8px;padding:1rem;margin:1rem 0">
<canvas id="particle-canvas" width="760" height="350" style="width:100%;border-radius:4px;cursor:none"></canvas>
</div>

<script>
(function(){
  const canvas = document.getElementById('particle-canvas');
  const ctx = canvas.getContext('2d');
  let mx=canvas.width/2, my=canvas.height/2;
  const particles=[];

  canvas.addEventListener('mousemove',e=>{
    const r=canvas.getBoundingClientRect();
    mx=(e.clientX-r.left)*(canvas.width/r.width);
    my=(e.clientY-r.top)*(canvas.height/r.height);
  });

  function spawn(){
    const angle=Math.random()*Math.PI*2;
    const speed=1+Math.random()*3;
    const life=30+Math.random()*60;
    const size=2+Math.random()*4;
    const hue=15+Math.random()*30; // orange range
    particles.push({x:mx,y:my,vx:Math.cos(angle)*speed,vy:Math.sin(angle)*speed-2,life,maxLife:life,size,hue});
  }

  function update(){
    ctx.fillStyle='rgba(26,26,46,0.15)';
    ctx.fillRect(0,0,canvas.width,canvas.height);

    for(let i=0;i<5;i++)spawn();

    for(let i=particles.length-1;i>=0;i--){
      const p=particles[i];
      p.x+=p.vx;p.y+=p.vy;p.vy+=0.05;p.life--;
      const alpha=p.life/p.maxLife;
      const s=p.size*alpha;
      ctx.fillStyle=`hsla(${p.hue},100%,${50+30*alpha}%,${alpha})`;
      ctx.fillRect(p.x-s/2,p.y-s/2,s,s);
      if(p.life<=0)particles.splice(i,1);
    }

    requestAnimationFrame(update);
  }
  update();
})();
</script>

---

## Spring Physics

Click and drag the ball. Watch it spring back with damped oscillation — the same math used for screen shake, camera follow, and juicy UI.

<div id="spring-demo" style="background:#1a1a2e;border-radius:8px;padding:1rem;margin:1rem 0">
<canvas id="spring-canvas" width="760" height="300" style="width:100%;border-radius:4px;cursor:grab"></canvas>
<div style="display:flex;gap:1rem;margin-top:8px;flex-wrap:wrap">
<label style="color:#888;font-size:12px">Stiffness: <input id="spring-k" type="range" min="0.01" max="0.5" step="0.01" value="0.08" style="width:100px"></label>
<label style="color:#888;font-size:12px">Damping: <input id="spring-d" type="range" min="0.8" max="0.99" step="0.01" value="0.92" style="width:100px"></label>
</div>
</div>

<script>
(function(){
  const canvas = document.getElementById('spring-canvas');
  const ctx = canvas.getContext('2d');
  const cx=canvas.width/2, cy=canvas.height/2;
  let bx=cx, by=cy, vx=0, vy=0;
  let dragging=false, trail=[];

  canvas.addEventListener('mousedown',e=>{dragging=true;canvas.style.cursor='grabbing';});
  canvas.addEventListener('mouseup',()=>{dragging=false;canvas.style.cursor='grab';});
  canvas.addEventListener('mousemove',e=>{
    if(!dragging)return;
    const r=canvas.getBoundingClientRect();
    bx=(e.clientX-r.left)*(canvas.width/r.width);
    by=(e.clientY-r.top)*(canvas.height/r.height);
    vx=0;vy=0;
  });

  function update(){
    const k=parseFloat(document.getElementById('spring-k').value);
    const d=parseFloat(document.getElementById('spring-d').value);

    if(!dragging){
      const dx=cx-bx, dy=cy-by;
      vx+=dx*k;vy+=dy*k;
      vx*=d;vy*=d;
      bx+=vx;by+=vy;
    }

    trail.push({x:bx,y:by});
    if(trail.length>40)trail.shift();

    ctx.fillStyle='#1a1a2e';ctx.fillRect(0,0,canvas.width,canvas.height);

    // Anchor
    ctx.strokeStyle='#333';ctx.lineWidth=2;
    ctx.beginPath();ctx.moveTo(cx,cy);ctx.lineTo(bx,by);ctx.stroke();
    ctx.fillStyle='#555';ctx.beginPath();ctx.arc(cx,cy,6,0,Math.PI*2);ctx.fill();

    // Trail
    trail.forEach((p,i)=>{
      const a=i/trail.length;
      ctx.fillStyle=`rgba(255,106,53,${a*0.3})`;
      ctx.beginPath();ctx.arc(p.x,p.y,8*a,0,Math.PI*2);ctx.fill();
    });

    // Ball
    ctx.fillStyle='#ff6a35';ctx.beginPath();ctx.arc(bx,by,14,0,Math.PI*2);ctx.fill();
    ctx.fillStyle='#ffa07a';ctx.beginPath();ctx.arc(bx-4,by-4,5,0,Math.PI*2);ctx.fill();

    requestAnimationFrame(update);
  }
  update();
})();
</script>

---

## Cellular Automata Cave Generation

Click "Generate" to create a new cave. Adjust iterations to see how the automata evolve.

<div id="cave-demo" style="background:#1a1a2e;border-radius:8px;padding:1rem;margin:1rem 0">
<canvas id="cave-canvas" width="760" height="380" style="width:100%;border-radius:4px"></canvas>
<div style="display:flex;gap:1rem;margin-top:8px;align-items:center;flex-wrap:wrap">
<button id="cave-gen" style="padding:4px 16px;background:#ff6a35;color:#fff;border:none;border-radius:4px;cursor:pointer;font-family:monospace">Generate</button>
<label style="color:#888;font-size:12px">Fill %: <input id="cave-fill" type="range" min="40" max="60" value="48" style="width:80px"><span id="cave-fill-val">48</span></label>
<label style="color:#888;font-size:12px">Iterations: <input id="cave-iter" type="range" min="1" max="10" value="5" style="width:80px"><span id="cave-iter-val">5</span></label>
</div>
</div>

<script>
(function(){
  const canvas = document.getElementById('cave-canvas');
  const ctx = canvas.getContext('2d');
  const W=95,H=47,S=8;

  function generate(){
    const fill=parseInt(document.getElementById('cave-fill').value);
    const iters=parseInt(document.getElementById('cave-iter').value);
    document.getElementById('cave-fill-val').textContent=fill;
    document.getElementById('cave-iter-val').textContent=iters;

    let grid=Array.from({length:H},(_,y)=>Array.from({length:W},(_,x)=>{
      if(x===0||y===0||x===W-1||y===H-1)return 1;
      return Math.random()*100<fill?1:0;
    }));

    for(let i=0;i<iters;i++){
      const next=grid.map(r=>[...r]);
      for(let y=1;y<H-1;y++)for(let x=1;x<W-1;x++){
        let walls=0;
        for(let dy=-1;dy<=1;dy++)for(let dx=-1;dx<=1;dx++){
          if(dx===0&&dy===0)continue;
          walls+=grid[y+dy][x+dx];
        }
        next[y][x]=walls>=5?1:walls<=2?0:grid[y][x];
      }
      grid=next;
    }

    ctx.fillStyle='#1a1a2e';ctx.fillRect(0,0,canvas.width,canvas.height);
    for(let y=0;y<H;y++)for(let x=0;x<W;x++){
      if(grid[y][x]){
        ctx.fillStyle='#3a3a5e';ctx.fillRect(x*S,y*S,S,S);
      }else{
        ctx.fillStyle='#8b6914';ctx.fillRect(x*S,y*S,S,S);
        ctx.fillStyle='#a07818';ctx.fillRect(x*S+1,y*S+1,S-2,S-2);
      }
    }
  }

  document.getElementById('cave-gen').onclick=generate;
  document.getElementById('cave-fill').oninput=generate;
  document.getElementById('cave-iter').oninput=generate;
  generate();
})();
</script>

---

## Tweening Playground

See how different tween properties animate a game object. Combines easing curves with real transforms.

<div id="tween-demo" style="background:#1a1a2e;border-radius:8px;padding:1rem;margin:1rem 0">
<canvas id="tween-canvas" width="760" height="200" style="width:100%;border-radius:4px"></canvas>
<div style="display:flex;gap:8px;margin-top:8px;flex-wrap:wrap">
<button class="tw-btn" data-prop="position" style="padding:2px 10px;font-size:12px;border:1px solid #ff6a35;background:#ff6a35;color:#fff;border-radius:3px;cursor:pointer;font-family:monospace">Position</button>
<button class="tw-btn" data-prop="scale" style="padding:2px 10px;font-size:12px;border:1px solid #444;background:#2a2a3e;color:#eee;border-radius:3px;cursor:pointer;font-family:monospace">Scale</button>
<button class="tw-btn" data-prop="rotation" style="padding:2px 10px;font-size:12px;border:1px solid #444;background:#2a2a3e;color:#eee;border-radius:3px;cursor:pointer;font-family:monospace">Rotation</button>
<button class="tw-btn" data-prop="color" style="padding:2px 10px;font-size:12px;border:1px solid #444;background:#2a2a3e;color:#eee;border-radius:3px;cursor:pointer;font-family:monospace">Color</button>
<button class="tw-btn" data-prop="all" style="padding:2px 10px;font-size:12px;border:1px solid #444;background:#2a2a3e;color:#eee;border-radius:3px;cursor:pointer;font-family:monospace">All</button>
</div>
</div>

<script>
(function(){
  const canvas = document.getElementById('tween-canvas');
  const ctx = canvas.getContext('2d');
  let prop='position', t=0;
  const backOut=t=>{const c=1.70158;return 1+(c+1)*Math.pow(t-1,3)+c*Math.pow(t-1,2);};

  document.querySelectorAll('.tw-btn').forEach(btn=>{
    btn.onclick=()=>{
      prop=btn.dataset.prop;t=0;
      document.querySelectorAll('.tw-btn').forEach(b=>{b.style.background='#2a2a3e';b.style.borderColor='#444';});
      btn.style.background='#ff6a35';btn.style.borderColor='#ff6a35';
    };
  });

  function draw(){
    ctx.fillStyle='#1a1a2e';ctx.fillRect(0,0,canvas.width,canvas.height);
    t+=0.005;if(t>1)t=0;
    const et=backOut(Math.min(t*2,1));

    let x=100,y=canvas.height/2,s=1,rot=0,r=255,g=106,b=53;

    if(prop==='position'||prop==='all')x=100+et*560;
    if(prop==='scale'||prop==='all')s=0.3+et*1.2;
    if(prop==='rotation'||prop==='all')rot=et*Math.PI*4;
    if(prop==='color'||prop==='all'){r=Math.floor(255-et*180);g=Math.floor(106+et*100);b=Math.floor(53+et*200);}

    // Ghost trail
    for(let i=0;i<5;i++){
      const tt=Math.max(0,t-i*0.03);
      const et2=backOut(Math.min(tt*2,1));
      let gx=100,gy=canvas.height/2;
      if(prop==='position'||prop==='all')gx=100+et2*560;
      ctx.fillStyle=`rgba(255,106,53,${0.05*(5-i)})`;
      ctx.fillRect(gx-15,gy-15,30,30);
    }

    ctx.save();
    ctx.translate(x,y);
    ctx.rotate(rot);
    ctx.scale(s,s);
    ctx.fillStyle=`rgb(${r},${g},${b})`;
    ctx.fillRect(-15,-15,30,30);
    ctx.strokeStyle='#fff3';
    ctx.strokeRect(-15,-15,30,30);
    ctx.restore();

    // Progress bar
    ctx.fillStyle='#333';ctx.fillRect(50,canvas.height-20,canvas.width-100,4);
    ctx.fillStyle='#ff6a35';ctx.fillRect(50,canvas.height-20,(canvas.width-100)*t,4);

    requestAnimationFrame(draw);
  }
  draw();
})();
</script>

---

All demos run entirely in-browser with vanilla JavaScript — no dependencies. The C# implementations in the [code examples](index.md) follow the same algorithms and patterns.
