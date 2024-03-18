using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using QuickQuiz.Repositories.Implementations;
using QuickQuiz.Repositories.Implementations.Participants;
using QuickQuiz.Repositories.Implementations.Setter;
using QuickQuiz.Repositories.Interfaces;
using QuickQuiz.Repositories.Interfaces.IParticipants;
using QuickQuiz.Repositories.Interfaces.ISetter;
using QuickQuiz.Services.Implementations;
using QuickQuiz.Services.Implementations.Participants;
using QuickQuiz.Services.Implementations.Setter;
using QuickQuiz.Services.Interfaces;
using QuickQuiz.Services.Interfaces.IParticipants;
using QuickQuiz.Services.Interfaces.ISetter;
using YourNamespace;

var builder = WebApplication.CreateBuilder(args);
// Add services to the container.


builder.Services.AddSingleton<IUserAuthRepository, UserAuthRepository>();
builder.Services.AddSingleton<IUserAuthService, UserAuthService>();
builder.Services.AddSingleton<IRoomService, RoomService>();
builder.Services.AddSingleton<IRoomRepository>(new RoomRepository(builder.Configuration["ConnectionStrings:DefaultConnection"]));
builder.Services.AddSingleton<IRoomPartRepository, RoomPartRepository>();
builder.Services.AddSingleton<IRoomPartService, RoomPartService>();
builder.Services.AddSingleton<ICustomLogger>(new CustomLogger(builder.Configuration["ConnectionStrings:DefaultConnection"]));
builder.Services.AddSingleton<IQuestionService, QuestionService>();
builder.Services.AddSingleton<IQuestionRepository>(new QuestionRepository(builder.Configuration["ConnectionStrings:DefaultConnection"]));
builder.Services.AddSingleton<IParticipantService, ParticipantService>();
builder.Services.AddSingleton<IParticipantRepository>(new ParticipantRepository(builder.Configuration["ConnectionStrings:DefaultConnection"]));



builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(
                builder.Configuration["Jwt:Key"]
            )),
            ClockSkew = TimeSpan.Zero
        };
    });


builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


var app = builder.Build();


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthentication();
app.UseAuthorization();
// app.UseMiddleware<IsActiveMiddleware>();
app.MapControllers();


// app.UseRouting();

app.UseCors(policy =>
{
    policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader();
});

// app.UseAuthorization();

// // app.MapControllers();

// app.UseEndpoints(endpoints =>
// {
//     endpoints.MapControllers();
// });

app.Run();