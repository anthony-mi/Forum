using Forum.Models.Entities;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;

namespace Forum.ViewModels
{
    public class EditTopicViewModel
    {
        public int Id { get; set; }

        public string Title { get; set; }

        public string Body { get; set; }

        public string SectionId { get; set; }

        public IList<SelectListItem> Sections { get; private set; }

        public bool IsCurrentUserModeratorOfSection { get; set; }

        public string Accessibility { get; set; }

        private IList<SelectListItem> _accessibilityTypes = null;

        public IList<SelectListItem> AccessibilityTypes
        {
            get
            {
                if(_accessibilityTypes == null)
                {
                    _accessibilityTypes = CreateAccessibilityTypes();
                    _accessibilityTypes[0].Selected = true;
                }

                return _accessibilityTypes;
            }

            private set
            {
                _accessibilityTypes = value;
            }
        }

        public EditTopicViewModel()
        {
            Accessibility = Models.Entities.Accessibility.FullAccess.ToString();
        }

        public EditTopicViewModel(Topic topic, IList<Section> sections, bool isCurrentUserModeratorOfSection)
        {
            Id = topic.Id;
            Title = topic.Title;
            Body = topic.Body;
            SectionId = topic.SectionId.ToString();
            Accessibility = topic.Accessibility.ToString();
            IsCurrentUserModeratorOfSection = isCurrentUserModeratorOfSection;

            Sections = new List<SelectListItem>();

            foreach (var section in sections)
            {
                var item = new SelectListItem(section.Name, section.Id.ToString(), section.Id == topic.SectionId);
                Sections.Add(item);
            }

            if(isCurrentUserModeratorOfSection)
            {
                AccessibilityTypes = new List<SelectListItem>();

                foreach (var value in Enum.GetValues(typeof(Accessibility)))
                {
                    var name = Enum.GetName(typeof(Accessibility), value);
                    var item = new SelectListItem(
                        name, 
                        value.ToString(), 
                        topic.Accessibility.ToString().Equals(value.ToString()));
                    AccessibilityTypes.Add(item);
                }
            }
        }

        private IList<SelectListItem> CreateAccessibilityTypes()
        {
            var types = new List<SelectListItem>();

            foreach (var value in Enum.GetValues(typeof(Accessibility)))
            {
                var name = Enum.GetName(typeof(Accessibility), value);
                var item = new SelectListItem(
                    name,
                    value.ToString());
                types.Add(item);
            }

            return types;
        }
    }
}
