using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using HMI.Models;

namespace HMI.Profiles
{
    public class NamesProfile : Profile
    {
        public NamesProfile()
        {
            CreateMap<Names, GetNamesDto>();
        }
    }
}
