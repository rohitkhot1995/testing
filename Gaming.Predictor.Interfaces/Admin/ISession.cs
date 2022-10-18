﻿using System;
using System.Collections.Generic;

namespace Gaming.Predictor.Interfaces.Admin
{
    public interface ISession
    {
        List<String> Pages(String name = "");

        bool _HasAdminCookie { get; }

        bool SetAdminCookie(String value);

        String SlideAdminCookie();

        void DeleteAdminCookie();
    }
}