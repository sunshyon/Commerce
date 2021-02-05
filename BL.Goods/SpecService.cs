using BL.Contracts;
using Domain.DbModels;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL.Goods
{
    public class SpecService:BaseService
    {
        private readonly OrangeContext _db;

        public SpecService(OrangeContext db)
        {
            _db = db;
        }
		public List<TbSpecGroup> QuerySpecGroupByCid(long cid)
		{
			TbSpecGroup specGroup = new TbSpecGroup();
			List<TbSpecGroup> specGroupList = _db.TbSpecGroup.Where(m => m.Cid == cid).ToList();
			if (specGroupList.Count <= 0)
			{
				throw new Exception("未找到规格组列表");
			}
			return specGroupList;
		}
		public List<TbSpecParam> QuerySpecParams(long? gid, long? cid, bool? searching, bool? generic)
		{
			
			var sql = new StringBuilder();
			sql.Append("select  id,cid,group_id,`name`,`numeric`,unit, generic, searching,segments,0 TbSpecGroupId from tb_spec_param where 1=1");
			if (gid != null)
			{
				sql.Append($" and group_id={gid} ");
			}
			if (cid != null)
			{
				sql.Append($" and cid={cid} ");
			}
			if (searching != null)
			{
				sql.Append($" and searching={searching} ");
			}
			if (generic != null)
			{
				sql.Append($" and generic={generic} ");
			}
			var aramList = _db.TbSpecParam.FromSqlRaw(sql.ToString()).ToList();

			List<TbSpecParam> specParamList = aramList.ToList();
			if (specParamList.Count <= 0)
			{
				throw new Exception("查询规则参数列表不存在");
			}
			return specParamList;
		}
		public List<TbSpecGroup> QuerySpecsByCid(long cid)
		{
			List<TbSpecGroup> specGroups = QuerySpecGroupByCid(cid);
			List<TbSpecParam> specParams = QuerySpecParams(null, cid, null, null);
			Dictionary<long?, List<TbSpecParam>> map = new Dictionary<long?, List<TbSpecParam>>();
			//遍历specParams
			foreach (TbSpecParam param in specParams)
			{
				var groupId = param.GroupId;
				if (!map.ContainsKey(param.GroupId))
				{
					//map中key不包含这个组ID
					map.Add(param.GroupId, new List<TbSpecParam>());
				}
				//添加进map中
				map[param.GroupId].Add(param);
			}
			foreach (TbSpecGroup specGroup in specGroups)
			{
				specGroup.Params.AddRange(map[specGroup.Id]);
			}
			return specGroups;
		}
        
	}
}
