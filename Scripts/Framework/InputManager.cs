using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class InputConfig 
{
   public KeyCode[] keyCodes { get; private set; }

   public InputConfig(KeyCode[] keyCodes)
   {
      this.keyCodes = keyCodes;
   }
      
}

[Serializable]
public enum ActionType 
{
   Up,
   Down,
   Left,
   Right,
   Attack,
   Jump,
   Dash,
   Skill,
   Switch,
   UltimateSkill,
   Help,
   Esc,
   Enter,
   Backpack,
   Interactive,
   None
}
   
public enum InputState
{
   Gaming,
   UI,
   Dialogue
}

public class InputManager : SingletonMono<InputManager>
{
   public VariableJoystick variableJoystick;
   public Dictionary<ActionType, UIInputButton> ButtonDic = new Dictionary<ActionType, UIInputButton>();
   InputManager() 
   {
       KeyCode[] keyCodes = {
          KeyCode.UpArrow, //Up
          KeyCode.DownArrow, //Down
          KeyCode.LeftArrow, //Left
          KeyCode.RightArrow, //Right
          KeyCode.J, //Attack
          KeyCode.Space, //Jump
          KeyCode.RightShift, //Dash
          KeyCode.K, //Skill
          KeyCode.L, //Switch
          KeyCode.J, //UltimateSkill
          KeyCode.G, //Help
          KeyCode.Escape, //Esc
          KeyCode.Return, //Enter
          KeyCode.B, //Backpack
          KeyCode.F  //Interactive
       };
       KeyCode[] keyCodes2 = {
          KeyCode.W, //Up
          KeyCode.S, //Down
          KeyCode.A, //Left
          KeyCode.D, //Right
          KeyCode.J, //Attack
          KeyCode.Space, //Jump
          KeyCode.LeftShift, //Dash
          KeyCode.K, //Skill
          KeyCode.L, //Switch
          KeyCode.J, //UltimateSkill
          KeyCode.G, //Help
          KeyCode.Escape, //Esc
          KeyCode.Return, //Enter
          KeyCode.B, //Backpack
          KeyCode.F  //Interactive
       };        
       //多套按键配置
       InputConfigDic.Add(0, new InputConfig(keyCodes));
       InputConfigDic.Add(1, new InputConfig(keyCodes2));
   }

   public bool canControl = true;
   public Dictionary<int, InputConfig> InputConfigDic = new Dictionary<int, InputConfig>();
   public InputState inputState = InputState.Gaming;
   private int lastPressX = 0;
   private int lastPressY = 0;
    
   public bool Up => CheckInput(ActionType.Up);
   public bool UpPress => CheckInputDown(ActionType.Up);
   public bool Down => CheckInput(ActionType.Down);
   public bool DownPress => CheckInputDown(ActionType.Down);
   public bool Left => CheckInput(ActionType.Left);
   public bool LeftPress => CheckInputDown(ActionType.Left);
   public bool Right => CheckInput(ActionType.Right);
   public bool RightPress => CheckInputDown(ActionType.Right);
   public bool Jump => CheckInput(ActionType.Jump);
   public bool JumpPress => CheckInputDown(ActionType.Jump);
   public bool Attack => CheckInput(ActionType.Attack);
   
   public bool AttackPress => CheckInputDown(ActionType.Attack);
   public bool DashPress => CheckInputDown(ActionType.Dash);
   public bool SkillPress => CheckInputDown(ActionType.Skill);
   public bool SwitchPress => CheckInputDown(ActionType.Switch);
   public bool UltimateSkillPress => CheckInputDown(ActionType.UltimateSkill);
   public bool HelpPress => CheckInputDown(ActionType.Help);
   public bool EscPress => CheckInputDown(ActionType.Esc);
   public bool EnterPress => CheckInputDown(ActionType.Enter);

    public bool BackpackPress => CheckInputDown(ActionType.Backpack); 
   public bool AnyKeyPress => CheckAnyKeyDown();

   public int HorizontalRaw 
   {
      get
      {
         if (!canControl && inputState == InputState.Gaming) return 0;
         if (Game.Instance.isGameOver) return 0;
#if UNITY_STANDALONE_WIN
         if (RightPress) lastPressX = 1;
         else if (LeftPress) lastPressX = -1;
         if (Right && !Left)
            return 1;
         else if (Left && !Right)
            return -1;
         else if (Left && Right)
            return lastPressX;
#endif

#if UNITY_ANDROID
         if (variableJoystick != null)
         {
            if (variableJoystick.Horizontal > 0.5f)
            {
               return 1;
            }
            else if (variableJoystick.Horizontal < -0.5f)
            {
               return -1;
            }
            else
            {
               return 0;
            }
         }
#endif
         
         return 0;
      }
   }
   public int VerticalRaw
   {
      get
      {
         if (!canControl && inputState == InputState.Gaming) return 0;
         if (Game.Instance.isGameOver) return 0;
#if UNITY_STANDALONE_WIN
         if (Up) lastPressY = 1;
         else if (Down) lastPressY = -1;
         if (Down && !Up)
            return -1;
         else if (Up && !Down)
            return 1;
         else if (Up && Down)
            return lastPressY;
#endif

#if UNITY_ANDROID
         if (variableJoystick != null)
         {
            if (variableJoystick.Vertical > 0.5f)
            {
               return 1;
            }
            else if (variableJoystick.Vertical < -0.5f)
            {
               return -1;
            }
            else
            {
               return 0;
            }
         }
#endif
         return 0;
      }
   }

   private bool CheckInput(ActionType index)
   {
#if UNITY_STANDALONE_WIN
      if (!canControl && inputState == InputState.Gaming && index != ActionType.Esc) return false;
      if (Game.Instance.isGameOver) return false;
      foreach (var item in InputConfigDic.Values) 
      {
         if (Input.GetKey(item.keyCodes[(int)index])) {
            return true;
         }
      }
      return false;
#endif
#if UNITY_ANDROID
      if (!canControl && inputState == InputState.Gaming) return false;
      if (Game.Instance.isGameOver) return false;
      if (ButtonDic.ContainsKey(index))
      {
         return ButtonDic[index].isTouch;
      }
      return false;
#endif
   }  
   
   private bool CheckInputDown(ActionType index) 
   {
      #if UNITY_STANDALONE_WIN
      if (!canControl && inputState == InputState.Gaming && index != ActionType.Esc) return false;
      if (Game.Instance.isGameOver) return false;
      foreach (var item in InputConfigDic.Values) 
      {
         if (Input.GetKeyDown(item.keyCodes[(int)index])) 
         {
            return true;
         }
      }
      #endif
      
      return false;
   }
   
   private bool CheckInputUp(ActionType index) 
   {
      #if UNITY_STANDALONE_WIN
      if (!canControl && inputState == InputState.Gaming && index != ActionType.Esc) return false;
      if (Game.Instance.isGameOver) return false;
      foreach (var item in InputConfigDic.Values) 
      {
         if (Input.GetKeyUp(item.keyCodes[(int)index])) 
         {
            return true;
         }
      }
      #endif
      
      return false;
   }

   private bool CheckAnyKeyDown()
   {
      if (!canControl || EscPress) return false;
      if (Game.Instance.isGameOver) return false;
      return Input.anyKeyDown;
   }
}
