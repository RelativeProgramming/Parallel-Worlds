using Normal.Realtime;
using UnityEngine;


public class FoodItem : RealtimeComponent<FoodItemModel>
{

    //protected override void OnRealtimeModelReplaced(FoodItemModel prevModel, FoodItemModel newModel)
    //{
    //    Debug.Log("FoodItem-Model: " + newModel.creator);
    //}

    public void SetCreator(string creator)
    {
        model.creator = creator;
    }

    public string GetCreator()
    {
        if (model == null) return "";
        return model.creator;
    }
}