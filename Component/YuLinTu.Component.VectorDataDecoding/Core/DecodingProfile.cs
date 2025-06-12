using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YuLinTu.Component.VectorDataDecoding.JsonEntity;
using YuLinTu.DF.Zones;

namespace YuLinTu.Component.VectorDataDecoding.Core
{
    internal class DecodingProfile : Profile
    {
        public DecodingProfile()
            {

            CreateMap<ZoneJsonEn, XZQH_XZDY>();
        }
    }
}
