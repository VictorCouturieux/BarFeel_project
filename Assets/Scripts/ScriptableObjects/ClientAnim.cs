using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "New ClientAnim", menuName = "ClientAnim")]
public class ClientAnim : ScriptableObject
{
    public CharacterType type;
    
    public Texture ClientBase;
    public Texture ClientDrink;
    public Texture ClientGrab;
    public Texture ClientSign;
}
