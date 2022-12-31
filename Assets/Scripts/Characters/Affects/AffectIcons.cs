using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

namespace characters
{
    public class AffectIcons : MonoBehaviour
    {
        Dictionary<Affect,GameObject> dict = new Dictionary<Affect, GameObject>();
        const float affectIconFontSize = 0.5f;
        const float affectTextBoxWidthAndHeightSize = 0.07f;

        public void UpdateAffectIcon(Affect affect)
        {
            foreach(KeyValuePair<Affect, GameObject> pair in dict)
            {
                if(pair.Key.GetType() == affect.GetType())
                {
                    if(affect.StackSize <= 0)
                    {
                        Destroy(pair.Value);
                        dict.Remove(affect);
                        return;
                    }
                    pair.Value.GetComponentInChildren<TextMeshPro>().text = "" + affect.StackSize;
                    return;
                }
            }
        }
        public void AddAffectIcon(Affect affect)
        {
            GameObject grid = this.GetComponentInChildren<GridLayoutGroup>().gameObject;

            //Image gameobject
            GameObject newIcon = new GameObject();

            newIcon.AddComponent<RectTransform>();
            newIcon.transform.SetParent(grid.transform);
            newIcon.transform.position = grid.transform.position;
            newIcon.transform.rotation = grid.transform.rotation;
            newIcon.transform.localScale = Vector3.one;
            newIcon.layer = grid.layer;
            newIcon.name = "" + affect.GetType();

            var renderer = newIcon.AddComponent<SpriteRenderer>();
            
            switch(affect.GetType())
            {
                case var value when value == typeof(Weaken):
                    renderer.sprite = Resources.Load<Sprite>("affect_weaken");
                break;
                case var value when value == typeof(Fracture):
                    renderer.sprite = Resources.Load<Sprite>("affect_fracture");
                break;
                case var value when value == typeof(Reinforce):
                    renderer.sprite = Resources.Load<Sprite>("affect_reinforce");
                break;
            }

            //Text gameobject
            GameObject textParent = new GameObject();
            textParent.transform.parent = newIcon.transform;

            TextMeshPro text3D = textParent.AddComponent<TextMeshPro>();
    
            text3D.transform.SetParent(newIcon.transform);
            text3D.transform.position = newIcon.transform.position;
            text3D.transform.rotation = newIcon.transform.rotation;
            text3D.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, affectTextBoxWidthAndHeightSize);
            text3D.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, affectTextBoxWidthAndHeightSize);
            text3D.transform.localScale = Vector3.one;

            text3D.font = cards.CardInfo.DEFAULT_FONT;
            text3D.fontSize = affectIconFontSize;
            text3D.name = "Stack Amount";
            text3D.text = "" + affect.StackSize;
            text3D.horizontalAlignment = HorizontalAlignmentOptions.Right;
            text3D.verticalAlignment = VerticalAlignmentOptions.Bottom;
            text3D.gameObject.layer = newIcon.layer;
            text3D.sortingOrder = 2;

            //Add to dict to update later
            dict.Add(affect, newIcon);
        }
    }
}
