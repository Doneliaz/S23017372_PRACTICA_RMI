using System;
using CoreWCF;

// La clase Operaciones contiene la lógica matemática
// Nota: En C# con CoreWCF no es obligatorio heredar de UnicastRemoteObject como en Java, 
// el framework maneja la exportación del objeto automáticamente.
public class Operaciones : IOperaciones
{
    // Constructor explícito incluyendo llamada a la clase padre (aunque Object es implícito en C#)
    public Operaciones() : base()
    {
    }

    public double Sumar(double numero1, double numero2)
    {
        return numero1 + numero2;
    }

    public double Restar(double numero1, double numero2)
    {
        return numero1 - numero2;
    }

    public double Multiplicar(double numero1, double numero2)
    {
        return numero1 * numero2;
    }

    public double Dividir(double numero1, double numero2)
    {
        // En Java se usaría throw new RemoteException(...), aquí usamos FaultException de WCF
        if (numero2 == 0)
        {
            throw new FaultException("No se puede dividir entre cero.");
        }
        return numero1 / numero2;
    }
}
