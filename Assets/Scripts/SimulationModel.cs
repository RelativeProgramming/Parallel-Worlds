using Normal.Realtime.Serialization;
using System.Collections;
using UnityEngine;
using Normal.Realtime;

[RealtimeModel]
public partial class SimulationModel
{
    [RealtimeProperty(1, true, true)]
    private RealtimeDictionary<SimulationUserModel> _users;
}

/* ----- Begin Normal Autogenerated Code ----- */
public partial class SimulationModel : RealtimeModel {
    public Normal.Realtime.Serialization.RealtimeDictionary<SimulationUserModel> users {
        get => _users;
    }
    
    public enum PropertyID : uint {
        Users = 1,
    }
    
    #region Properties
    
    private ModelProperty<Normal.Realtime.Serialization.RealtimeDictionary<SimulationUserModel>> _usersProperty;
    
    #endregion
    
    public SimulationModel() : base(null) {
        RealtimeModel[] childModels = new RealtimeModel[1];
        
        _users = new Normal.Realtime.Serialization.RealtimeDictionary<SimulationUserModel>();
        childModels[0] = _users;
        
        SetChildren(childModels);
        
        _usersProperty = new ModelProperty<Normal.Realtime.Serialization.RealtimeDictionary<SimulationUserModel>>(1, _users);
    }
    
    protected override int WriteLength(StreamContext context) {
        var length = 0;
        length += _usersProperty.WriteLength(context);
        return length;
    }
    
    protected override void Write(WriteStream stream, StreamContext context) {
        var writes = false;
        writes |= _usersProperty.Write(stream, context);
        if (writes) InvalidateContextLength(context);
    }
    
    protected override void Read(ReadStream stream, StreamContext context) {
        var anyPropertiesChanged = false;
        while (stream.ReadNextPropertyID(out uint propertyID)) {
            var changed = false;
            switch (propertyID) {
                case (uint) PropertyID.Users: {
                    changed = _usersProperty.Read(stream, context);
                    break;
                }
                default: {
                    stream.SkipProperty();
                    break;
                }
            }
            anyPropertiesChanged |= changed;
        }
        if (anyPropertiesChanged) {
            UpdateBackingFields();
        }
    }
    
    private void UpdateBackingFields() {
        _users = users;
    }
    
}
/* ----- End Normal Autogenerated Code ----- */