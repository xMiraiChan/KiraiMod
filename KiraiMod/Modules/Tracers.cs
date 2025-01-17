﻿using UnityEngine;
using VRC.SDKBase;
using BepInEx.Configuration;

namespace KiraiMod.Modules
{
    public static class Tracers
    {
        public static ConfigEntry<bool> PlayerTracers = Plugin.cfg.Bind(nameof(Tracers), "Players", false, "Should lines be drawn to every player in the world");

        private static LineRenderer lr;

        static Tracers()
        {
            Core.UI.LegacyGUIManager.OnLoad += () =>
            {
                Core.UI.UIGroup ui = new(nameof(Tracers));
                ui.RegisterAsHighest();
                ui.AddElement("Player Tracers", PlayerTracers.Value).Bound.Bind(PlayerTracers);
            };


            Events.ApplicationStart += () =>
            {
                SetupLineRenderer(lr = new GameObject("KiraiMod.Tracers").DontDestroyOnLoad().AddComponent<LineRenderer>(), new Color32(0x56, 0x00, 0xA5, 0xAF));
                PlayerTracers.SettingChanged += ((System.EventHandler)((sender, args) =>
                {
                    if (lr.gameObject.active = PlayerTracers.Value)
                        Events.Update += UpdatePlayerTracers;
                    else Events.Update -= UpdatePlayerTracers;
                })).Invoke();
            };
        }

        public static void UpdatePlayerTracers()
        {
            if (Networking.LocalPlayer is null) return;

            lr.positionCount = VRCPlayerApi.AllPlayers.Count * 2;
            Draw(lr, Networking.LocalPlayer.GetTrackingData(VRCPlayerApi.TrackingDataType.RightHand).position);
        }

        private static void SetupLineRenderer(LineRenderer lr, Color color)
        {
            lr.material = new Material(Shader.Find("Legacy Shaders/Particles/Additive"));
            lr.startWidth = 0.002f;
            lr.endWidth = 0.002f;
            lr.useWorldSpace = false;
            lr.endColor = color;
            lr.startColor = color;
        }

        private static void Draw(LineRenderer lr, Vector3 src)
        {
            for (int i = 0; i < VRCPlayerApi.AllPlayers.Count; i++)
            {
                lr.SetPosition(i * 2, src);
                lr.SetPosition(i * 2 + 1, VRCPlayerApi.AllPlayers[i].gameObject.transform.position);
            }
        }
    }
}
