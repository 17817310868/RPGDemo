﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameDesigner
{
    public class NodeStart : MonoBehaviour
    {
        public BlueprintNode node;

        void Start()
        {
            node.Invoke();
        }
    }
}
