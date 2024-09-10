using API;
using API.Middleware;

var builder = WebApplication.CreateBuilder(args);


builder.AddServices();
builder.AddDatabase();
builder.AddValidations();
builder.AddMapper();
builder.AddSwaggerDocs();
builder.AddJwtAuth();
builder.AddInjections();
builder.AddRepositories();
builder.Services.AddHttpContextAccessor();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}


app.UseHttpsRedirection();
app.UseMiddleware<ExceptionMiddleware>();
app.UseAuthorization();
app.MapControllers();
app.Run();
