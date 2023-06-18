using System;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;

namespace Ensemble
{
    public class Predicate
    {
        public bool? IsBoolean { get; set; }
        public bool? CloneEachTimeStep { get; set; }
        public bool? Active { get; set; }
        public bool Actionable { get; set; }

        public string Operator { get; set; }

        public dynamic DefaultValue { get; set; }
        public dynamic Value { get; set; }
        public dynamic IntentType { get; set; }

        public int? MinValue { get; set; }
        public int? MaxValue { get; set; }
        public int? Duration { get; set; }
        public int? Weight { get; set; }
        public int? Order { get; set; }
        public int TimeHappened { get; set; }

        public List<Condition> Conditions { get; set; }
        public List<Effect> Effects { get; set; }
        public List<EnglishData> EnglishInfluences { get; set; }

        public List<string> Labels { get; set; }
        public List<string> TurnsAgoBetween { get; set; }
        public List<string> Types { get; set; }

        public string Category { get; set; }
        public string Direction { get; set; }
        public string DirectionType { get; set; }
        public string First { get; set; }
        public string ID { get; set; }
        public string Name { get; set; }
        public string Origin { get; set; }
        public string Second { get; set; }
        public string Type { get; set; }

        private void setPredicate(string category, string type, string first, string second, string operation, dynamic value, List<string> turnsAgoBetween, int? order, dynamic intentType)
        {
            this.Category = category;
            this.Type = type;
            this.First = first;
            this.Second = second;
            this.Operator = operation;

            if (value is bool)
            {
                this.Value = value ? 1 : 0;
                this.IsBoolean = true;
            }
            else if (value is int)
            {
                this.Value = value;
                this.IsBoolean = false;
            }
            else
            {
                this.Value = null;
                this.IsBoolean = null;
            }

            this.Order = order;
            this.TurnsAgoBetween = turnsAgoBetween;
            this.IntentType = intentType;

            this.CloneEachTimeStep = null;
            this.Active = null;
            this.MinValue = null;
            this.MaxValue = null;
            this.Duration = null;
            this.Weight = null;
            this.Order = null;
        }

        public Predicate() { }

        public Predicate(string category, string type)
        {
            this.setPredicate(category, type, null, null, null, null, null, null, null);
        }

        public Predicate(string category, string type, string first)
        {
            this.setPredicate(category, type, first, null, null, null, null, null, null);
        }

        public Predicate(string category, string type, string first, int value)
        {
            this.setPredicate(category, type, first, null, null, value, null, null, null);
        }

        public Predicate(string category, string type, string first, bool value)
        {
            this.setPredicate(category, type, first, null, null, value, null, null, null);
        }

        public Predicate(string category, string type, string first, List<string> turnsAgoBetween)
        {
            this.setPredicate(category, type, first, null, null, null, turnsAgoBetween, null, null);
        }

        public Predicate(string category, string type, string first, bool value, List<string> turnsAgoBetween)
        {
            this.setPredicate(category, type, first, null, null, value, turnsAgoBetween, null, null);
        }

        public Predicate(string category, string type, string first, int value, string operation)
        {
            this.setPredicate(category, type, first, null, operation, value, null, null, null);
        }

        public Predicate(string category, string type, string first, int value, bool intentType)
        {
            this.setPredicate(category, type, first, null, null, value, null, null, intentType);
        }

        public Predicate(string category, string type, string first, string second)
        {
            this.setPredicate(category, type, first, second, null, null, null, null, null);
        }

        public Predicate(string category, string type, string first, string second, int value)
        {
            this.setPredicate(category, type, first, second, null, value, null, null, null);
        }

        public Predicate(string category, string type, string first, string second, int value, bool intentType)
        {
            this.setPredicate(category, type, first, second, null, value, null, null, intentType);
        }

        public Predicate(string category, string type, string first, string second, bool value)
        {
            this.setPredicate(category, type, first, second, null, value, null, null, null);
        }

        public Predicate(string category, string type, string first, string second, bool value, List<string> turnsAgoBetween)
        {
            this.setPredicate(category, type, first, second, null, value, turnsAgoBetween, null, null);
        }

        public Predicate(string category, string type, string first, string second, List<string> turnsAgoBetween, int order)
        {
            this.setPredicate(category, type, first, second, null, null, turnsAgoBetween, order, null);
        }

        public Predicate(string category, string type, string first, string second, int value, string operation)
        {
            this.setPredicate(category, type, first, second, operation, value, null, null, null);
        }

        public Predicate(string category, string type, string first, string second, int value, string operation, List<string> turnsAgoBetween)
        {
            this.setPredicate(category, type, first, second, operation, value, turnsAgoBetween, null, null);
        }

        public Predicate(string category, string type, string first, string second, string operation, int value, List<string> turnsAgoBetween, int order)
        {
            this.setPredicate(category, type, first, second, operation, value, turnsAgoBetween, order, null);
        }

        public Predicate ShallowCopy()
        {
            return (Predicate)this.MemberwiseClone();
        }

        public Predicate _DeepCopy()
        {
            Predicate deepCopy = (Predicate)this.MemberwiseClone();

            if (Category != null)
            {
                deepCopy.Category = String.Copy(Category);
            }
            
            if (DirectionType != null)
            {
                deepCopy.DirectionType = String.Copy(DirectionType);
            }
            
            if (Type != null)
            {
                deepCopy.Type = String.Copy(Type);
            }

            if (First != null)
            {
                deepCopy.First = String.Copy(First);
            }
            
            if (Second != null)
            {
                deepCopy.Second = String.Copy(Second);
            }
            
            if (Origin != null)
            {
                deepCopy.Origin = String.Copy(Origin);
            }

            if (ID != null)
            {
                deepCopy.ID = String.Copy(ID);
            }
            
            if (Name != null)
            {
                deepCopy.Name = String.Copy(Name);
            }

            if (Types != null)
            {
                deepCopy.Types = Types.Select(type => (string)type.Clone()).ToList();
            }

            if (Conditions != null)
            {
                deepCopy.Conditions = Conditions.Select(condition => (Condition)condition.DeepCopy()).ToList();
            }

            if (Effects != null)
            {
                deepCopy.Effects = Effects.Select(effect => (Effect)effect.DeepCopy()).ToList();
            }
            
            if (EnglishInfluences != null)
            {
                deepCopy.EnglishInfluences = EnglishInfluences.Select(influence => (EnglishData)influence.DeepCopy()).ToList();
            }

            if (TurnsAgoBetween != null)
            {
                deepCopy.TurnsAgoBetween = TurnsAgoBetween.Select(turns => (string)turns.Clone()).ToList();
            }

            return deepCopy;
        }

        public Predicate DeepCopy()
        {
            return _DeepCopy();
        }

        public override string ToString()
        {
            String predToString = "";

            if (this.Category != null) { predToString += String.Format("Category: {0}", this.Category.ToString()); }
            if (this.Type != null) { predToString += String.Format(", Type: {0}", this.Type.ToString()); }
            if (this.Value != null) { predToString += String.Format(", Value: {0}", this.Value.ToString()); }
            if (this.IsBoolean != null) { predToString += String.Format(", IsBoolean: {0}", this.IsBoolean); }
            if (this.First != null) { predToString += String.Format(", First: {0}", this.First); }
            if (this.Second != null) { predToString += String.Format(", Second: {0}", this.Second); }
            if (this.Duration != null) { predToString += String.Format(", Duration: {0}", this.Duration); }
            if (this.Operator != null) { predToString += String.Format(", Operator: {0}", this.Operator); }
            if (this.Weight != null) { predToString += String.Format(", Weight: {0}", this.Weight); }

            return predToString;
        }

        public override bool Equals(object obj)
        {
            return this.Equals(obj as Predicate);
        }

        public bool ExistingPredicate(Predicate other)
        {
            if (other == null)
                return false;

            return
                this.Category == other.Category &&
                this.Type == other.Type &&
                this.First == other.First &&
                this.Second == other.Second;
        }

        public bool Equals(Predicate other)
        {
            if (other == null)
                return false;

            return
                this.Category == other.Category &&
                this.Type == other.Type &&
                this.First == other.First &&
                this.Second == other.Second &&
                this.Operator == other.Operator &&
                this.Value == other.Value &&
                this.IsBoolean == other.IsBoolean &&
                this.Order == other.Order &&
                this.TurnsAgoBetween == other.TurnsAgoBetween &&
                this.IntentType == other.IntentType &&
                this.CloneEachTimeStep == other.CloneEachTimeStep &&
                this.Active == other.Active &&
                this.MinValue == other.MinValue &&
                this.MaxValue == other.MaxValue &&
                this.Duration == other.Duration &&
                this.Weight == other.Weight &&
                this.Order == other.Order;
        }
    }
}
