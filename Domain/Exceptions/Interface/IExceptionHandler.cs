using Domain.Data.Models;

namespace Domain.Exceptions.Interface;

public interface IExceptionHandler
{
    /// <summary>
    /// Processa uma exceção e gera uma resposta de erro padronizada.
    /// </summary>
    /// <param name="exception">A exceção a ser tratada.</param>
    /// <returns>Um objeto <see cref="ErrorResponse"/> contendo detalhes sobre o erro, como código de status e mensagem.</returns>
    ErrorResponse Handle(System.Exception exception);
}
