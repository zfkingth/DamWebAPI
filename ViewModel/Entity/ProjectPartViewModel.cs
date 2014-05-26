using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using DamServiceV3.Test.DamServiceRef;

namespace DamWebAPI.ViewModel
{
    public class ProjectPartViewModel : ViewModelBase
    {

        public ProjectPartViewModel(ProjectPartViewModel parent, ProjectPart entity, Func<ProjectPartViewModel, ObservableCollection<ProjectPartViewModel>> retrieveChildrenFunc)
        {
            ParentViewModel = parent;
            _entity = entity;
            RetrieveChildrenFunc = retrieveChildrenFunc;
        }

        public ProjectPartViewModel ParentViewModel { get; set; }

        private  ProjectPart _entity;

        public ProjectPart Entity
        {
            get { return _entity; }
            set { _entity = value; }
        }


        public override string ToString()
        {
            return PartName;
        }


        public System.Guid ProjectPartID
        {
            get
            {
                return _entity.Id;
            }
        }




        public string PartName
        {
            get { return _entity.PartName; }
            set
            {
                if (_entity.PartName != value)
                {
                    _entity.PartName = value;
                    RaisePropertyChanged("PartName");
                }
            }
        }





        public Guid? ParentPartID
        {
            get { return _entity.ParentPart; }
            set
            {
                if (_entity.ParentPart != value)
                {
                    _entity.ParentPart = value;
                    RaisePropertyChanged("ParentPart");
                }
            }
        }



        public Func<ProjectPartViewModel, ObservableCollection<ProjectPartViewModel>> RetrieveChildrenFunc;

        private ObservableCollection<ProjectPartViewModel> _children = null;
        public ObservableCollection<ProjectPartViewModel> Children
        {
            get
            {
                if (_children == null)
                {
                    _children = RetrieveChildrenFunc(this);
                }
                return _children;
            }
            set
            {
                if (_children != value)
                {
                    _children = value;
                    RaisePropertyChanged("Children");
                }
            }
        }




    }
}
