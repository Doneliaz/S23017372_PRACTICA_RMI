using System.ServiceModel;

// La interfaz indica qué métodos puede invocar el cliente de manera remota.
// [ServiceContract] es el equivalente en C# a heredar de java.rmi.Remote
[ServiceContract]
public interface IOperaciones
{
    // [OperationContract] indica que el método se puede acceder por la red
    [OperationContract]
    double Sumar(double numero1, double numero2);

    [OperationContract]
    double Restar(double numero1, double numero2);

    [OperationContract]
    double Multiplicar(double numero1, double numero2);

    [OperationContract]
    double Dividir(double numero1, double numero2);
}
