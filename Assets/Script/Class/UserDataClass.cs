using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UserDataClass
{
    public int gold;
    public int reputation;

    public List<bool> ingredientUnlock; // 재료 해금여부
    public List<bool> recipeUnlock; // 레시피 해금여부
    public List<bool> machineUnlock; // 기계 해금여부

    public UserDataClass(){
        gold = 0;
        reputation = 0;

        ingredientUnlock = new List<bool>();
        ingredientUnlock.Clear();
        for(int i=0; i<14; i++){
            switch(i){
                case 3: // 얼음
                case 6: // 자몽청
                case 8: // 콜드 브루
                    ingredientUnlock.Add(false);
                    break;
                default:
                    ingredientUnlock.Add(true);
                    break;
            }
        }

        recipeUnlock = new List<bool>();
        recipeUnlock.Clear();
        for(int i=0; i<21; i++){
            if((i>5 && i<12) || i==17 || i==20){ 
                recipeUnlock.Add(false);
            }
            else{
                recipeUnlock.Add(true);
            }
        }

        machineUnlock = new List<bool>();
        machineUnlock.Clear();
        for(int i=0; i<7; i++){
            machineUnlock.Add(false);
        }

    }

}
