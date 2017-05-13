﻿using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using JoyOI.Blog.Models;

namespace JoyOI.Blog.Controllers
{
    public class BaseController : BaseController<BlogContext, User, Guid>
    {
        public User SiteOwner { get; private set; }

        public override void Prepare()
        {
            base.Prepare();

            SiteOwner = DB.DomainBindings
                .SingleOrDefault(x => x.Domain == HttpContext.Request.Host.Host)
                ?.User;

            if (SiteOwner == null)
            {
                HttpContext.Response.Redirect("http://www.joyoi.net");
                return;
            }

            // Building Constants
            ViewBag.Position = "home";
            ViewBag.IsPost = false;
            ViewBag.Description = SiteOwner.Summary;
            ViewBag.Title = SiteOwner.SiteName;
            ViewBag.AboutUrl = "/about";
            ViewBag.AvatarUrl = "/download/" + SiteOwner.AvatarId;
            ViewBag.Account = SiteOwner.Nickname;
            ViewBag.DefaultTemplate = SiteOwner.Template;

            // Building Tags
            ViewBag.Tags = DB.PostTags
                .OrderBy(x => x.Tag)
                .GroupBy(x => x.Tag)
                .Select(x => new TagViewModel
                {
                    Title = x.Key,
                    Count = x.Count()
                })
                .ToList();

            // Building Calendar
            ViewBag.Calendars = DB.Posts
                .Where(x => !x.IsPage)
                .OrderByDescending(x => x.Time)
                .GroupBy(x => new { Year = x.Time.Year, Month = x.Time.Month })
                .Select(x => new CalendarViewModel
                {
                    Year = x.Key.Year,
                    Month = x.Key.Month,
                    Count = x.Count()
                })
                .ToList();

            // Building Catalogs
            ViewBag.Catalogs = DB.Catalogs
                .Include(x => x.Posts)
                .OrderByDescending(x => x.PRI)
                .ToList()
                .Select(x => new CatalogViewModel
                {
                    Id = x.Id,
                    Title = x.Title,
                    Count = x.Posts.Count(),
                    PRI = x.PRI,
                    Url = x.Url
                })
                .ToList();

            // Building Blog Rolls
            var rolls = DB.BlogRolls
                .Where(x => !string.IsNullOrEmpty(x.URL) && x.AvatarId.HasValue)
                .OrderByDescending(x => x.Type)
                .Select(x => new BlogRollViewModel
                {
                    AvatarId = x.AvatarId.Value,
                    Name = x.NickName,
                    URL = x.URL
                })
                .ToList();
            rolls.Reverse();
            ViewBag.Rolls = rolls;
        }
    }
}
