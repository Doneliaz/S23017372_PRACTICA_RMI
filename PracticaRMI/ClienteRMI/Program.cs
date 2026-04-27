using System;
using System.Collections.Generic;
using System.ServiceModel;
using System.Runtime.Serialization;

namespace ClienteRMI
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("--- Cliente RMI de Prueba ---");
            Console.WriteLine("Intentando conectar al Servidor...");
            
            // Configuramos la conexión hacia el Servidor RMI (TCP Local sin seguridad)
            var binding = new NetTcpBinding(SecurityMode.None);
            var endpoint = new EndpointAddress("net.tcp://localhost:1099/PersonaController");
            
            // ChannelFactory es el equivalente en WCF a Naming.lookup("rmi://...") de Java
            var factory = new ChannelFactory<IPersonaController>(binding, endpoint);
            IPersonaController servidorPersonas = factory.CreateChannel();

            try
            {
                // -- 1. Listar --
                Console.WriteLine("\n[1] Solicitando lista de personas...");
                List<Persona> listaPersonas = servidorPersonas.List();
                Console.WriteLine($"Se recuperaron {listaPersonas.Count} registros de la Base de Datos:\n");
                
                foreach (Persona p in listaPersonas)
                {
                    Console.WriteLine($"  - ID: {p.IdPersona} | Nombre: {p.Nombre} | Email: {p.Email}");
                }

                // -- 2. Probar Find(Persona) (Nueva Funcionalidad) --
                Console.WriteLine("\n[2] Probando Find(Persona) buscando por Email...");
                Persona buscarPersona = new Persona();
                buscarPersona.Email = "alex@mail.com";
                List<Persona> encontrados = servidorPersonas.Find(buscarPersona);
                Console.WriteLine($"Se encontraron {encontrados.Count} coincidencias para 'alex@mail.com':");
                foreach (var p in encontrados)
                {
                    Console.WriteLine($"  - ID: {p.IdPersona} | Nombre: {p.Nombre} | Email: {p.Email}");
                }

                // -- 3. Probar Update --
                Console.WriteLine("\n[3] Probando Update()...");
                Persona updatePersona = new Persona { IdPersona = 1, Nombre = "Beto (Actualizado)", Email = "beto_nuevo@mail.com", Telefono = "123456789" };
                int resUpdate = servidorPersonas.Update(updatePersona);
                if (resUpdate == IPersonaController.UPDATE_EXITO) Console.WriteLine("  ¡Actualizado con éxito!");
                else if (resUpdate == IPersonaController.UPDATE_ID_INEXISTENTE) Console.WriteLine("  El ID no existe.");
                else Console.WriteLine("  Error al actualizar. Código: " + resUpdate);

                // -- 4. Probar Delete(int) (El Reto) --
                int idEliminar = 6;
                Console.WriteLine($"\n[4] Probando DeleteById(int) para eliminar ID {idEliminar}...");
                int resultadoDelete = servidorPersonas.DeleteById(idEliminar);
                if (resultadoDelete == IPersonaController.DELETE_EXITO)
                    Console.WriteLine("  ¡Eliminado con éxito!");
                else if (resultadoDelete == IPersonaController.DELETE_ID_INEXISTENTE)
                    Console.WriteLine("  El ID no existe.");
                else
                    Console.WriteLine("  Error al eliminar. Código: " + resultadoDelete);
            }
            catch (Exception ex)
            {
                Console.WriteLine("\nError al conectar o recuperar datos. ¿El ServidorRMI está encendido?");
                Console.WriteLine("Detalle: " + ex.Message);
            }
            finally
            {
                // Cerramos los canales de red
                if (servidorPersonas is IClientChannel channel)
                {
                    channel.Close();
                }
                factory.Close();
            }

            Console.WriteLine("\nPresiona Enter para salir...");
            Console.ReadLine();
        }
    }
}
