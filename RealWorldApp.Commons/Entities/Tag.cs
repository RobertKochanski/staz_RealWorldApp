﻿namespace RealWorldApp.Commons.Entities
{
    public class Tag : BaseEntitie
    {
        public string Name { get; set; }
        public List<Article> Articles { get; set; }
    }
}
