using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class VisualEffectFunctionalTests
{
    private GameObject effectObject;

    [SetUp]
    public void SetUp()
    {
        effectObject = new GameObject("Test_VisualEffect");

        VisualEffect effect =
            effectObject.AddComponent<VisualEffect>();

        effect.destructionTime = 0.2f;
    }

    [TearDown]
    public void TearDown()
    {
        if (effectObject != null)
            Object.DestroyImmediate(effectObject);
    }

    [UnityTest]
    public IEnumerator VisualEffect_RemainsActive_BeforeDestructionTime()
    {
        VisualEffect effect =
            effectObject.GetComponent<VisualEffect>();

        effect.destructionTime = 0.5f;

        effectObject.SetActive(false);
        effectObject.SetActive(true);

        yield return new WaitForSeconds(0.1f);

        Assert.IsNotNull(
            effectObject,
            "Visual effect should still exist before destructionTime is reached.");
    }

    [UnityTest]
    public IEnumerator VisualEffect_IsDestroyed_AfterDestructionTime()
    {
        VisualEffect effect =
            effectObject.GetComponent<VisualEffect>();

        effect.destructionTime = 0.1f;

        effectObject.SetActive(false);
        effectObject.SetActive(true);

        yield return new WaitForSeconds(0.2f);

        Assert.IsTrue(
            effectObject == null,
            "Visual effect should be destroyed after destructionTime.");
    }

    [UnityTest]
    public IEnumerator VisualEffect_StartsTimer_WhenEnabled()
    {
        VisualEffect effect =
            effectObject.GetComponent<VisualEffect>();

        effect.destructionTime = 0.1f;

        effectObject.SetActive(false);

        Assert.IsFalse(effectObject.activeSelf);

        effectObject.SetActive(true);

        yield return new WaitForSeconds(0.2f);

        Assert.IsTrue(
            effectObject == null,
            "Enabling the object should start its destruction timer.");
    }
}