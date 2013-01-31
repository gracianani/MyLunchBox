using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MyLunchBox.Models
{
    public enum SiteMenu
    {
        Home,
        Create,
        Cart,
        Account,
        Help
    }

    public class SiteMenuHelper
    {
        private static SiteMenuHelper instance;


        private SiteMenuHelper()
        {
            _currentSiteMenu = SiteMenu.Home;
        }

        public static SiteMenuHelper Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new SiteMenuHelper();
                }
                return instance;
            }
        }

        private SiteMenu _currentSiteMenu;
        public SiteMenu CurrentSiteMenu
        {
            get { return _currentSiteMenu; }
            set { _currentSiteMenu = value; }
        }
    }
}