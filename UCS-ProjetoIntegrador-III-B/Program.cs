using UCS_ProjetoIntegrador_III_B.Data;
using UCS_ProjetoIntegrador_III_B.Repositories;
using UCS_ProjetoIntegrador_III_B.Repositories.Interfaces;
using UCS_ProjetoIntegrador_III_B.Services;
using UCS_ProjetoIntegrador_III_B.Services.Interfaces;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllersWithViews();
builder.Services.AddScoped<IEmpresaService, EmpresaService>();
builder.Services.AddScoped<IAlunoService, AlunoService>();
builder.Services.AddScoped<IOrientadorService, OrientadorService>();
builder.Services.AddScoped<IEstagioService, EstagioService>();
builder.Services.AddSingleton<DatabaseService>();
builder.Services.AddScoped<IEmpresaRepository, EmpresaRepository>();
builder.Services.AddScoped<IAlunoRepository, AlunoRepository>();
builder.Services.AddScoped<IOrientadorRepository, OrientadorRepository>();
builder.Services.AddScoped<IEstagioRepository, EstagioRepository>();

var app = builder.Build();
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();
app.UseAuthorization();
app.MapStaticAssets();
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();
app.Run();
