using System;

namespace Ape.Volo.Common.DI;

public interface IDisposableContainer : IDisposable
{
    void AddDisposableObj(IDisposable disposableObj);
}
