﻿using Senparc.Areas.Admin.ACL.Repository;
using Senparc.Ncf.Core.Models.DataBaseModel;
using Senparc.Ncf.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Senparc.Areas.Admin.Domain.Services
{
    public class SysRoleService : BaseClientService<SysRole>
    {
        public SysRoleService(ClientRepositoryBase<SysRole> repo, IServiceProvider serviceProvider) : base(repo, serviceProvider)
        {
        }
    }

    /// <summary>
    /// 菜单
    /// </summary>
    public class SysMenuService : BaseClientService<SysMenu>
    {
        private readonly ISysMenuRepository _repo;
        public SysMenuService(ISysMenuRepository repo, IServiceProvider serviceProvider) : base(repo, serviceProvider)
        {
            _repo = repo;
        }

        /// <summary>
        /// 获取所有菜单(不包含页面)
        /// TODO... 缓存
        /// </summary>
        /// <returns></returns>
        public async Task<IEnumerable<SysMenuTreeItemDto>> GetAllMenusTreeAsync(bool hasButton)
        {
            var allMenus = await _repo.GetAllMenuDtosAsync(hasButton);
            IList<SysMenuTreeItemDto> items = new List<SysMenuTreeItemDto>(allMenus.Count() / 2);
            var iteration = allMenus.Where(_ => _.ParentId == null || _.ParentId == string.Empty).OrderByDescending(o => o.Sort);
            buildTreeItems(iteration, items, allMenus);
            return items;
        }

        /// <summary>
        /// 递归获取树形结构
        /// </summary>
        /// <param name="iteration"></param>
        /// <param name="items"></param>
        /// <param name="source"></param>
        private void buildTreeItems(IEnumerable<SysMenuDto> iteration, IList<SysMenuTreeItemDto> items, IEnumerable<SysMenuDto> source)
        {
            foreach (var menu in iteration)
            {
                var childs = source.Where(_ => _.ParentId == menu.Id).OrderByDescending(o => o.Sort);
                if (!childs.Any())
                {
                    return;
                }
                var p = new SysMenuTreeItemDto()
                {
                    Children = new List<SysMenuTreeItemDto>(childs.Count()),
                    Icon = menu.Icon,
                    Id = menu.Id,
                    IsMenu = menu.MenuType == MenuType.菜单,
                    MenuName = menu.MenuName,
                    Url = menu.Url
                };
                items.Add(p);
                buildTreeItems(childs, items, source);
            }
        }
    }
}
