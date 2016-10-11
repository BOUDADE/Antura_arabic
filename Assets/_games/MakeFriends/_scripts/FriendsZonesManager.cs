﻿using UnityEngine;
using System.Collections;

namespace EA4S.MakeFriends
{
    public class FriendsZonesManager : MonoBehaviour
    {
        public FriendsZone[] zones;
        [HideInInspector]
        public FriendsZone currentZone;

        public static FriendsZonesManager instance;

        private int currentZoneIndex;


        void Awake()
        {
            instance = this;
        }

        void Start()
        {
            currentZoneIndex = 0;
            currentZone = zones[currentZoneIndex];
        }

        public void IncrementCurrentZone()
        {
            if (currentZoneIndex >= zones.Length - 1)
            {
                Debug.Log("No more Friends Zones!");
                return;
            }
            currentZoneIndex++;
            currentZone = zones[currentZoneIndex];
        }
    }
}