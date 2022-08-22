namespace dhc;

public interface IHandlingInvoker<T>
{
    Task Handle(T context, ContextDelegate<T> next);
}
