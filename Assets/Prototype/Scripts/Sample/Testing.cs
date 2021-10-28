using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;


public class Testing : MonoBehaviour
{
    private void Start() {
        Task.Run(async () =>
        {
            int o = await Foo();
            Debug.Log($"return {o}");

        });


    }

    private async Task<int> Foo() {
        return await Task<int>.Run(() =>
        {
            Thread.Sleep(1000);
            return 1;

        });
    }

    private void t1()
    {
        SemaphoreSlim semaphore = new SemaphoreSlim(0, 1);
        Task.Run(() => {
            Debug.Log("start waiting");
            semaphore.Wait();
            Debug.Log("end waiting");
        });
        Thread.Sleep(3000);
        Debug.Log("Releas 1 semaphore");
        semaphore.Release(1);
    }

}
