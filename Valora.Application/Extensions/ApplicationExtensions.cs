using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using Valora.Application.Behaviors;
using Valora.Application.UseCases.Items.Create; 

namespace Valora.Application.Extensions;

public static class ApplicationExtensions
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        var assembly = typeof(CreateItemCommand).Assembly;

        services.AddMediatR(cfg =>
        {
            cfg.LicenseKey = "eyJhbGciOiJSUzI1NiIsImtpZCI6Ikx1Y2t5UGVubnlTb2Z0d2FyZUxpY2Vuc2VLZXkvYmJiMTNhY2I1OTkwNGQ4OWI0Y2IxYzg1ZjA4OGNjZjkiLCJ0eXAiOiJKV1QifQ.eyJpc3MiOiJodHRwczovL2x1Y2t5cGVubnlzb2Z0d2FyZS5jb20iLCJhdWQiOiJMdWNreVBlbm55U29mdHdhcmUiLCJleHAiOiIxODAyOTA4ODAwIiwiaWF0IjoiMTc3MTQyNTYzMSIsImFjY291bnRfaWQiOiIwMTljNzEzMGYxOGU3M2YwYmFiMzc4MDRjNWVkNDU0YiIsImN1c3RvbWVyX2lkIjoiY3RtXzAxa2hyazQzOTZieXhzdnJqc3FyMnhmOHc2Iiwic3ViX2lkIjoiLSIsImVkaXRpb24iOiIwIiwidHlwZSI6IjIifQ.WK4LNMd8ohVMLnAW8_M6cjSMuzxnuMiogeqdzhbFP4gIHjInjNpHgqNzTIQEcY4WAmzyvhriLuNRRtaVgpMGY4U-CO4cFjDTP_wryEOBr3Ue58rNUIhfepZZfzJSUt05ODL8iKzE9ww60PVwXFG_xVllZsckIZway45BVWlg73r2OoNtBTUQBLmM0ltNiblslu5FPz5CxyezKiZ4Govi8Vy9e8OwxOoOlei2fgcIr3VdqLSxhSpsurkFBETI9Lqr3ceVw9YVMKbL-e7wp9qGUbLh27BNP_V1vH6pNhwmRyePyTTRV8XFrUCKf3GETw6ypp3UtIxVFpLwpD5JGlgHoA";
            cfg.RegisterServicesFromAssembly(assembly);
            cfg.AddOpenBehavior(typeof(ValidationBehavior<,>));
        });

        services.AddValidatorsFromAssembly(assembly);

        return services;
    }
}