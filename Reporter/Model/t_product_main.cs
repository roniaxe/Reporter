//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Reporter.Model
{
    using System;
    using System.Collections.Generic;
    
    public partial class t_product_main
    {
        public short product_no { get; set; }
        public string name { get; set; }
        public string short_name { get; set; }
        public Nullable<short> product_group { get; set; }
        public Nullable<short> commission_calc_base { get; set; }
        public Nullable<short> cash_value_flag { get; set; }
        public Nullable<short> contribution_oriented_flag { get; set; }
        public Nullable<short> medical_declaration_type { get; set; }
        public Nullable<short> policy_sa_parameter { get; set; }
        public Nullable<short> sa_auto_indexation { get; set; }
        public Nullable<short> premium_auto_indexation { get; set; }
        public short insufficient_units_method { get; set; }
        public short initial_debt_allowed_flag { get; set; }
        public Nullable<short> investment_type { get; set; }
        public Nullable<short> product_family { get; set; }
        public Nullable<short> group_product_code { get; set; }
        public Nullable<short> compliance_format { get; set; }
        public Nullable<short> allocation_method { get; set; }
        public Nullable<short> main_commission_method { get; set; }
        public string product_account_for_gl { get; set; }
        public Nullable<short> product_sub_group { get; set; }
        public Nullable<int> last_updated_user_id { get; set; }
        public Nullable<System.DateTime> last_updated_reg_date { get; set; }
        public string contract_official_name { get; set; }
        public Nullable<short> transfer_product_code { get; set; }
        public string legal_scheme_name { get; set; }
        public short anniversary_indexation_level { get; set; }
        public short valuation_compliance_format { get; set; }
        public short commission_payable_method { get; set; }
        public string policy_an_letter { get; set; }
        public Nullable<short> policy_an_counter { get; set; }
        public Nullable<short> policy_no_method { get; set; }
        public Nullable<short> product_account_code_group { get; set; }
        public short default_product { get; set; }
        public short default_product_flag { get; set; }
        public short active_product_flag { get; set; }
        public short manual_agent_layer_on_cover { get; set; }
        public int uniqid { get; set; }
        public short agent_property_retrieve_mode { get; set; }
        public int discriminator { get; set; }
    }
}
