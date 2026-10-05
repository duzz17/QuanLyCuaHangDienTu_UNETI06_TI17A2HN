const fs=require('fs');
const cssPath='wwwroot/css/site.css';
const css=fs.readFileSync(cssPath,'utf8').replace(/^\uFEFF/,'');
let depth=0;
const tokens=css.match(/\/\*[\s\S]*?\*\/|[^{};]+|[{};]/g)||[];
let lines=[],current='';
const flush=()=>{const line=current.trim();if(line)lines.push('  '.repeat(depth)+line);current='';};
for(const token of tokens){
 if(token.startsWith('/*')){flush();lines.push('  '.repeat(depth)+token.trim());}
 else if(token==='{'){current=current.trim().replace(/,(?![^()]*\))/g,', ')+' {';flush();depth++;}
 else if(token==='}'){flush();depth=Math.max(0,depth-1);lines.push('  '.repeat(depth)+'}');if(depth===0)lines.push('');}
 else if(token===';'){current+=';';flush();}
 else current+=token;
}
flush();
fs.writeFileSync(cssPath,lines.join('\n').replace(/\n{3,}/g,'\n\n')+'\n','utf8');
// Split adjacent HTML elements to keep Razor views reviewable without changing expressions.
for(const file of ['Views/Dashboard/Index.cshtml','Views/Home/Index.cshtml','Views/SanPham/Index.cshtml','Views/SanPham/Details.cshtml','Views/GioHang/Index.cshtml','Views/DonHang/Index.cshtml','Views/DonHang/Details.cshtml','Views/DonHang/DatHang.cshtml','Views/TaiKhoan/Login.cshtml','Views/Shared/_Layout.cshtml','Views/Shared/_ProductCard.cshtml','Views/Shared/_ProductFilters.cshtml','Views/Shared/_CartTotals.cshtml','Views/Shared/_CheckoutSteps.cshtml']){
 const text=fs.readFileSync(file,'utf8').replace(/^\uFEFF/,'').replace(/></g,'>\n<');
 fs.writeFileSync(file,text,'utf8');
}

