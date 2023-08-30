using System.Collections.Generic;
using BLL.Models.Auth;
using BLL.Models.Post.Request;

namespace JoyFusionInitializer.Models;

public class InitializeUserModel : RegisterModel
{
    public  PostCreateRequest[] PostModels { get; set; }
}