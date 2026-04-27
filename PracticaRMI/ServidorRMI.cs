using System;
using CoreWCF;
using CoreWCF.Configuration;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;

public class ServidorRMI
{
    public static void Main(string[] args)
    {
        // El builder preparará el entorno en el que se ejecuta nuestro servidor.
        var builder = WebApplication.CreateBuilder(args);

        // Equivalente a LocateRegistry.createRegistry(1099)
        // Se abre el puerto de red en localhost para empezar a escuchar peticiones.
        builder.WebHost.UseNetTcp(1099);

        // Agregar los servicios del framework (CoreWCF) al contenedor
        builder.Services.AddServiceModelServices();

        var app = builder.Build();

        // Equivalente a Naming.rebind(...)
        app.UseServiceModel(b =>
        {
            // --- 1. Servicio de Operaciones Matemáticas ---
            b.AddService<Operaciones>();
            var binding = new NetTcpBinding(SecurityMode.None); 
            b.AddServiceEndpoint<Operaciones, IOperaciones>(
                binding, 
                "net.tcp://localhost:1099/Operaciones"
            );

            // --- 2. Nuevo Servicio de Personas ---
            b.AddService<PersonaController>();
            b.AddServiceEndpoint<PersonaController, IPersonaController>(
                binding, 
                "net.tcp://localhost:1099/PersonaController"
            );
        });

        Console.WriteLine("Servidor RMI/WCF listo...");
        Console.WriteLine("Exponiendo:");
        Console.WriteLine("- 'Operaciones' en net.tcp://localhost:1099/Operaciones");
        Console.WriteLine("- 'PersonaController' en net.tcp://localhost:1099/PersonaController");
        Console.WriteLine("En espera de que algún cliente se conecte...");

        // Arrancar el servicio y mantenerlo vivo
        app.Run();
    }
}
