using System.Collections.Generic;
using BLL.Models.Auth;
using BLL.Models.Post;

namespace JoyFusionInitializer.Models;

public class InitializeUserModel : RegisterModel
{
    public PostCreateModel[] PostCreateModels { get; set; }
}