//===--------------------------------------------------------------===//
// Copyright (C) 2021-2023 mingmoe(me@kawayi.moe)(https://kawayi.moe)
// 
// This file is licensed under the MIT license.
// MIT LICENSE:https://opensource.org/licenses/MIT
//
//===--------------------------------------------------------------===//
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utopia.Core;

namespace Utopia.Test
{
    class GuuidTest
    {

        [Test]
        public void TestGuuidToStringAndParseStringWorksWell()
        {
            var guuid = new Guuid("root","node");
            var str = guuid.ToString();

            var parsed = Guuid.ParseString(str);

            Assert.That(parsed, Is.EqualTo(guuid));
        }




    }
}
