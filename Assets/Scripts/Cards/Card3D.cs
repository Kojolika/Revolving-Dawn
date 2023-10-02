using UnityEngine;
using TMPro;
using System.Collections.Generic;
using Characters;
using FightDamageCalc;
using Mana;

namespace Cards
{
    public class Card3D : MonoBehaviour
    {
        [SerializeField] Card _cardScriptableObject; //card object where all the data is retrieved from (the view part of MVC)

        public Card CardScriptableObject
        {
            get => _cardScriptableObject;
            set
            {
                _cardScriptableObject = value;
                PopulateFromData();
            }
        }
        [SerializeField] new TextMeshPro name;
        [SerializeField] TextMeshPro description;
        [SerializeField] SpriteRenderer artwork;
        [SerializeField] SpriteRenderer border;
        [SerializeField] GameObject manaSockets; //parent object to instantiate sockets under
        [SerializeField] GameObject socketPrefab; //socket prefab to instatiate depending on the cards mana
        [SerializeField] Mana3D[] manaInSockets; //mana currently in this cards sockets, start as null for every socket

        [SerializeField] ParticleSystem playableOutline;
        [SerializeField] ParticleSystem flash;

        public static float CAMERA_DISTANCE => Camera.main.nearClipPlane + 7;
        public static Vector3 DEFAULT_SCALE => new Vector3(0.2f, 1f, 0.3f);

        public void Play(List<Character> targets) => _cardScriptableObject.Play(targets);
        public Targeting GetTarget() => _cardScriptableObject.target;
        public Character Owner { get => _cardScriptableObject.owner; set => _cardScriptableObject.owner = value; }

        //color of card outline
        ParticleSystem.MinMaxGradient cachedGradient;
        void OnEnable()
        {
            FightInput.PlayerTurnInputManager.StaticInstance.MouseEnterPlayArea += EnteredPlayArea;
            FightInput.PlayerTurnInputManager.StaticInstance.MouseEnterMana3D += MousedOverMana;
            FightInput.PlayerTurnInputManager.StaticInstance.MouseExitMana3D += MouseLeftMana;
            FightInput.PlayerTurnInputManager.StaticInstance.RegisterCardEvents(this);
            fight.FightEvents.OnCharacterTurnAction += PlayPlayableOutlineParticles;
            fight.FightEvents.OnCharacterTurnEnded += PausePlayableOutlineParticles;

            //Cache color of outline for later
            var playableOutlineColorOverTime = playableOutline.colorOverLifetime;
            cachedGradient = playableOutlineColorOverTime.color;
        }
        void OnDisable()
        {
            FightInput.PlayerTurnInputManager.StaticInstance.MouseEnterPlayArea -= EnteredPlayArea;
            FightInput.PlayerTurnInputManager.StaticInstance.MouseEnterMana3D -= MousedOverMana;
            FightInput.PlayerTurnInputManager.StaticInstance.MouseExitMana3D -= MouseLeftMana;
            FightInput.PlayerTurnInputManager.StaticInstance.UnregisterCardEvents(this);
            fight.FightEvents.OnCharacterTurnAction -= PlayPlayableOutlineParticles;
            fight.FightEvents.OnCharacterTurnEnded -= PausePlayableOutlineParticles;
        }

        public void PopulateFromData()
        {
            if (_cardScriptableObject == null) return;

            artwork.sprite = _cardScriptableObject.artwork;
            border.sprite = CardConfiguration.GetClassBorder(_cardScriptableObject.@class);

            name.text = _cardScriptableObject.name;
            name.font = CardConfiguration.DEFAULT_FONT;
            name.color = CardConfiguration.DEFAULT_FONT_COLOR;
            name.fontSize = CardConfiguration.DEFAULT_FONT_NAME_SIZE;

            description.text = _cardScriptableObject.descriptionWithReplaceables;
            description.font = CardConfiguration.DEFAULT_FONT;
            description.color = CardConfiguration.DEFAULT_FONT_COLOR;
            description.fontSize = CardConfiguration.DEFAULT_FONT_NAME_SIZE;
            UpdateDescription(null);

            Owner = _cardScriptableObject.owner;

            manaInSockets = new Mana3D[_cardScriptableObject.mana.Length];

            //If sockets are already instantiated, destroy them
            if (manaSockets.transform.childCount > 0)
            {
                for (int index = 0; index < manaSockets.transform.childCount; index++)
                {
                    Destroy(manaSockets.transform.GetChild(index));
                }
            }

            for (int index = 0; index < _cardScriptableObject.mana.Length; index++)
            {
                //instantiate the socket from the prefab
                GameObject instanitatedSocket = Instantiate(socketPrefab, manaSockets.transform, false);

                //offset each socket by a little amount (makes a column on the left side of the card)
                instanitatedSocket.transform.position += new Vector3(0f, -0.45f, 0f) * index;

                //set the color of the socket to the manas color
                instanitatedSocket.transform.GetChild(0).GetComponent<Renderer>().sharedMaterial = ManaConfiguration.GetManaColor(_cardScriptableObject.mana[index]);

            }


        }
        public void UpdateDescription(Character target)
        {
            ProcessingChain chain = new ProcessingChain();
            var numberValues = _cardScriptableObject.numberValues;
            for (int index = 0; index < numberValues.Count; index++)
            {
                Number copy = numberValues[index];
                float numberAffectProcessing = chain.Process(copy, _cardScriptableObject.owner, target).Amount;
                string typeOfNumberAndIndex = copy.GetDamageType().ToString().ToUpper() + (index + 1);

                if (numberAffectProcessing < copy.Amount)
                {
                    description.text = _cardScriptableObject.descriptionWithReplaceables.Replace(typeOfNumberAndIndex, "<color=#910505>" + numberAffectProcessing + "</color>");
                }
                else if (numberAffectProcessing > copy.Amount)
                {
                    description.text = _cardScriptableObject.descriptionWithReplaceables.Replace(typeOfNumberAndIndex, "<color=#00FF00>" + numberAffectProcessing + "</color>");
                }
                else
                {
                    description.text = _cardScriptableObject.descriptionWithReplaceables.Replace(typeOfNumberAndIndex, "" + numberAffectProcessing);
                }

            }
        }
        //Summary:
        //Transform card to next or previous in chain
        //
        //Args:
        //direction: true is forward transform (typically an upgrade to the card)
        //           false is backwards transform
        public void Transform(bool direction)
        {
            if (_cardScriptableObject.Transform(direction) == null) return;

            _cardScriptableObject = _cardScriptableObject.Transform(direction);

            RemoveSockets();
            PopulateFromData();
        }
        public bool BindMana(Mana3D manaBeingBound)
        {
            bool readyToTransform = true;
            bool binded = false;

            for (int index = 0; index < manaInSockets.Length; index++)
            {
                //manaInSockets and cardScriptableObject.mana will always be same length
                if (manaInSockets[index] == null && manaBeingBound.type == _cardScriptableObject.mana[index])
                {
                    Transform socketTransform = manaSockets.transform.GetChild(index);
                    FitManaIntoSocket(manaBeingBound, socketTransform);
                    binded = true;
                    manaInSockets[index] = manaBeingBound;
                    continue;
                }

                if (manaInSockets[index] == null) readyToTransform = false;
            }

            //input is true because when every mana is fit into the socket, the card is upgraded
            if (readyToTransform) Transform(true);

            void FitManaIntoSocket(Mana3D mana, Transform socket)
            {
                mana.transform.SetParent(socket, false);
                mana.transform.localPosition = Vector3.zero;
                mana.transform.localScale = new Vector3(.05f, .05f, .05f);
            }

            return binded;
        }
        public List<ManaType> UnBindAndReturnMana()
        {
            List<ManaType> manaTypesToBeUnbound = new List<ManaType>();
            foreach (Mana3D mana in manaInSockets)
            {
                if (mana != null)
                {
                    manaTypesToBeUnbound.Add(mana.type);
                }
            }

            return manaTypesToBeUnbound;
        }

        void RemoveSockets()
        {
            for (int index = 0; index < manaSockets.transform.childCount; index++)
            {
                Destroy(manaSockets.transform.GetChild(index).gameObject);
            }
        }

        public delegate void MouseOver(Card3D card);
        public event MouseOver OnMouseOverEvent;

        //OnMouseEnter is a unity event that is called whenever the mouse first enters a gameobject with a collider
        //OnMouseEnterEvent is my custom event that now lets me subscribe other functions to the event in response to a mouse entering this card
        void OnMouseOver()
        {
            if (OnMouseOverEvent != null)
            {
                OnMouseOverEvent(this);
            }
        }
        public delegate void MouseExit(Card3D card);
        public event MouseExit OnMouseExitEvent;

        void OnMouseExit()
        {
            if (OnMouseExitEvent != null)
            {
                OnMouseExitEvent(this);
            }
        }
        void PlayPlayableOutlineParticles(Character character)
        {
            if (character != Owner) return;

            playableOutline.Play();
        }
        void PausePlayableOutlineParticles(Character character)
        {
            if (character != Owner) return;

            playableOutline.Pause();
        }
        void EnteredPlayArea()
        {
            //Hacky way to tell if this card is the selected card
            //Only the selected card would have a dragger
            if (this.TryGetComponent<Dragger>(out Dragger dragger))
            {
                PlayFlash(Color.cyan);
            }
        }


        void MousedOverMana(Mana3D mana)
        {
            ManaType type = mana.type;
            foreach (ManaType manaType in this.CardScriptableObject.mana)
            {
                if (manaType == type)
                {
                    ChangeColorOfPlayableParticleOutline(type);
                    PlayFlash(ColorForManaType(type));
                }
                
            }
        }

        void MouseLeftMana()
        {
            var playableOutlineColorOverTime = playableOutline.colorOverLifetime;
            playableOutlineColorOverTime.color = cachedGradient;
        }
        void ChangeColorOfPlayableParticleOutline(ManaType manaType)
        {
            var playableOutlineColorOverTime = playableOutline.colorOverLifetime;

            Gradient newGradient = new Gradient();

            Color color = ColorForManaType(manaType);

            newGradient.SetKeys(new GradientColorKey[]{
                new GradientColorKey(color, 0.0f),
                new GradientColorKey(color, 1.0f)
            }, new GradientAlphaKey[] {
                new GradientAlphaKey(1f, 0.0f), // fade out
                new GradientAlphaKey(0.0f, 1.0f) //
            });

            playableOutlineColorOverTime.color = newGradient;


        }
        void PlayFlash(Color color)
        {
            var flashMain = flash.main;
            flashMain.startColor = color;

            var flashColor = flash.colorOverLifetime;
            Gradient newGradient = new Gradient();

            newGradient.SetKeys(new GradientColorKey[]{
                new GradientColorKey(color, 0.0f),
                new GradientColorKey(color, 1.0f)
            }, new GradientAlphaKey[] {
                new GradientAlphaKey(0.5f, 0.0f), // fade out
                new GradientAlphaKey(0.0f, 1.0f) //
            });

            flashColor.color = newGradient;

            flash.Play();
        }
        Color ColorForManaType(ManaType manaType)
        {
            switch (manaType)
            {
                case ManaType.Red:
                    return Color.red;
                case ManaType.Blue:
                    return Color.blue;
                case ManaType.Green:
                    return Color.green;
                case ManaType.White:
                    return Color.white;
                case ManaType.Gold:
                    return Color.yellow;
                case ManaType.Black:
                    return Color.black;
            }
            return Color.cyan;
        }
    }
}
