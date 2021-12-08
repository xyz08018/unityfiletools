using System.Collections.Generic;

public class tos_default
{
    List<string> param_name = new List<string>();
    List<string> param_value = new List<string>();

    public static tos_default New()
    {
        return new tos_default();
    }

    public string ToString()
    {
        string s = "";
        for (int i = 0; i < param_name.Count; i++)
        {
            if (0 == i)
                s += param_name[i] + "=" + param_value[i];
            else
                s += "&" + param_name[i] + "=" + param_value[i];
        }
        return s;
    }


    public tos_default AddPhone(string phone_number)
    {
        param_name.Add("phone");
        param_value.Add(phone_number);
        return this;
    }

    public tos_default AddRegType(string regType)
    {
        param_name.Add("regType");
        param_value.Add(regType);
        return this;
    }

    public tos_default AddRoute(string route)
    {
        param_name.Add("route");
        param_value.Add(route);
        return this;
    }

    public tos_default AddParkId(string parkId)
    {
        param_name.Add("parkId");
        param_value.Add(parkId);
        return this;
    }

    public tos_default AddToken(string token)
    {
        param_name.Add("token");
        param_value.Add(token);
        return this;
    }

    public tos_default AddTypeId(string typeId)
    {
        param_name.Add("typeId");
        param_value.Add(typeId);
        return this;
    }

    public tos_default AddProjectId(string projectId)
    {
        param_name.Add("projectId");
        param_value.Add(projectId);
        return this;
    }

    public tos_default AddRoomId(string roomId)
    {
        param_name.Add("roomId");
        param_value.Add(roomId);
        return this;
    }

    public tos_default AddMobileType(string mobileType)
    {
        param_name.Add("mobileType");
        param_value.Add(mobileType);
        return this;
    }
}
