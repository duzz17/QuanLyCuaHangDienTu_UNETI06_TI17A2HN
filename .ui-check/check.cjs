const { chromium } = require('C:/Users/Admin/AppData/Local/OpenAI/Codex/runtimes/cua_node/f1bf3cd3a5929acd/bin/node_modules/playwright');
const path = require('path');
(async () => {
 const browser = await chromium.launch({executablePath:'C:/Program Files (x86)/Microsoft/Edge/Application/msedge.exe', headless:true, args:['--no-sandbox','--disable-gpu']});
 const context = await browser.newContext({viewport:{width:1440,height:1000},locale:'vi-VN'});
 const page = await context.newPage(); const errors=[];
 page.on('pageerror', e=>errors.push(e.message));
 for (const [name,url] of [['home','/'],['products','/SanPham'],['login','/TaiKhoan/Login'],['empty-cart','/GioHang'],['dashboard-auth','/Dashboard']]) {
  const response=await page.goto('http://localhost:5126'+url,{waitUntil:'networkidle'});
  console.log(JSON.stringify({name,status:response.status(),url:page.url(),title:await page.title(),h1:await page.locator('h1').allTextContents(),overflow:await page.evaluate(()=>document.documentElement.scrollWidth>innerWidth)}));
  if(name!=='dashboard-auth') await page.screenshot({path:path.join(__dirname,name+'-desktop.png'),fullPage:true});
 }
 await page.goto('http://localhost:5126/SanPham',{waitUntil:'networkidle'});
 const firstDetails=await page.locator('.product-image').first().getAttribute('href');
 await page.goto('http://localhost:5126'+firstDetails,{waitUntil:'networkidle'});
 console.log(JSON.stringify({name:'product-details',title:await page.title(),h1:await page.locator('h1').textContent()}));
 await page.screenshot({path:path.join(__dirname,'details-desktop.png'),fullPage:true});
 await page.locator('[data-quantity-step="1"]').click();
 if(await page.locator('[name="soLuong"]').inputValue()!=='2') throw new Error('Quantity increment failed');
 await page.locator('.purchase-form button[type="submit"]').click();
 await page.waitForURL('**/GioHang*');
 console.log(JSON.stringify({name:'cart-add',rows:await page.locator('.cart-item').count(),quantity:await page.locator('.cart-quantity-form input[name="soLuong"]').first().inputValue()}));
 await page.screenshot({path:path.join(__dirname,'cart-desktop.png'),fullPage:true});
 await page.setViewportSize({width:390,height:844});
 for(const [name,url] of [['home','/'],['products','/SanPham'],['cart','/GioHang'],['login','/TaiKhoan/Login']]) {
  await page.goto('http://localhost:5126'+url,{waitUntil:'networkidle'});
  console.log(JSON.stringify({name:name+'-mobile',overflow:await page.evaluate(()=>document.documentElement.scrollWidth>innerWidth)}));
  await page.screenshot({path:path.join(__dirname,name+'-mobile.png'),fullPage:true});
 }
 await page.goto('http://localhost:5126/SanPham?tuKhoa=zzzz-no-products',{waitUntil:'networkidle'});
 console.log(JSON.stringify({name:'empty-search',empty:await page.locator('.empty-state').count()}));
 await page.goto('http://localhost:5126/SanPham?thuongHieu=Apple&sapXep=gia_asc&page=2',{waitUntil:'networkidle'});
 console.log(JSON.stringify({name:'preserved-filters',selected:await page.locator('#filter-brand').inputValue(),pagination:await page.locator('.pagination a').evaluateAll(els=>els.map(e=>e.getAttribute('href')))}));
 await page.goto('http://localhost:5126/DonHang/DatHang',{waitUntil:'networkidle'});
 console.log(JSON.stringify({name:'checkout-auth',url:page.url()}));
 console.log(JSON.stringify({pageErrors:errors}));
 await browser.close();
})().catch(e=>{console.error(e);process.exit(1)});
