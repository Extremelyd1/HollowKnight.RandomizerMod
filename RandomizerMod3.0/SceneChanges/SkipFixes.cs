using RandomizerMod.Components;
using RandomizerMod.Extensions;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace RandomizerMod.SceneChanges
{
    internal static partial class SceneEditor
    {
        public static void FixMiscSkips(Scene newScene)
        {
            switch (newScene.name)
            {
                // Make Cliffs shade skip feasible
                case SceneNames.Fungus1_28:
                    GameObject cliffsCrawlid = Object.Instantiate(GameObject.Find("Crawler"));
                    cliffsCrawlid.SetActive(true);
                    cliffsCrawlid.transform.position = new Vector2(74f, 31f);
                    if (RandomizerMod.Instance.Settings.ShadeSkips && RandomizerMod.Instance.Settings.SpicySkips && PlayerData.instance.hasDoubleJump && !PlayerData.instance.hasWalljump)
                    {
                        foreach (GameObject g in GameManager.FindObjectsOfType<GameObject>())
                        {
                            if (g.transform.GetPositionX() < 75 && g.transform.GetPositionX() > 70 && g.transform.GetPositionY() < 54 && g.transform.GetPositionY() > 33)
                            {
                                Object.Destroy(g);
                            }
                        }
                    }
                    break;

                // Make descending dark spikes pogoable
                case SceneNames.Mines_35:
                    foreach (NonBouncer nonBounce in Object.FindObjectsOfType<NonBouncer>())
                    {
                        if (nonBounce.gameObject.name.StartsWith("Spike Collider"))
                        {
                            nonBounce.active = false;
                            nonBounce.gameObject.AddComponent<RandomizerTinkEffect>();
                        }
                    }
                    break;

                // Wings-only pogo to Soul Sanctum
                case SceneNames.Ruins1_05:
                    GameObject chandelier = newScene.FindGameObject("ruind_dressing_light_02 (10)");
                    chandelier.transform.SetPositionX(chandelier.transform.position.x - 2);
                    chandelier.GetComponent<NonBouncer>().active = false;
                    break;

                // Sign pogo to get past right-side city without items
                case SceneNames.Ruins2_04:
                    {
                        GameObject plat = ObjectCache.SmallPlatform;
                        plat.SetActive(true);
                        plat.transform.position = new Vector2(18f, 10f);
                    }
                    break;

                // Respawn jars in Collector's room to allow wings only access
                case SceneNames.Ruins2_11:
                    SavePersistentBoolItemState(new PersistentBoolData
                    {
                        sceneName = "Ruins2_11",
                        id = "Break Jar",
                        activated = false,
                        semiPersistent = false
                    });
                    SavePersistentBoolItemState(new PersistentBoolData
                    {
                        sceneName = "Ruins2_11",
                        id = "Break Jar (1)",
                        activated = false,
                        semiPersistent = false
                    });
                    SavePersistentBoolItemState(new PersistentBoolData
                    {
                        sceneName = "Ruins2_11",
                        id = "Break Jar (2)",
                        activated = false,
                        semiPersistent = false
                    });
                    SavePersistentBoolItemState(new PersistentBoolData
                    {
                        sceneName = "Ruins2_11",
                        id = "Break Jar (3)",
                        activated = false,
                        semiPersistent = false
                    });
                    SavePersistentBoolItemState(new PersistentBoolData
                    {
                        sceneName = "Ruins2_11",
                        id = "Break Jar (4)",
                        activated = false,
                        semiPersistent = false
                    });
                    SavePersistentBoolItemState(new PersistentBoolData
                    {
                        sceneName = "Ruins2_11",
                        id = "Break Jar (5)",
                        activated = false,
                        semiPersistent = false
                    });
                    SavePersistentBoolItemState(new PersistentBoolData
                    {
                        sceneName = "Ruins2_11",
                        id = "Break Jar (6)",
                        activated = false,
                        semiPersistent = false
                    });
                    SavePersistentBoolItemState(new PersistentBoolData
                    {
                        sceneName = "Ruins2_11",
                        id = "Break Jar (7)",
                        activated = false,
                        semiPersistent = false
                    });
                    SavePersistentBoolItemState(new PersistentBoolData
                    {
                        sceneName = "Ruins2_11",
                        id = "Break Jar (8)",
                        activated = false,
                        semiPersistent = false
                    });
                    break;
            }
        }
    }
}
