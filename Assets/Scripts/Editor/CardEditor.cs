using UnityEditor;
using Cards;


// Prevents "Argument Exception: Getting control 1's position in a group with only 1 controls when doing repaint" error
//
// http://answers.unity.com/answers/1678113/view.html
//
[CustomEditor(typeof(Card),true)]
public class CardEditor : Editor
{
}

